using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerOfLove.Base
{
    public class Sprite
    {
        #region Attributes
        Dictionary<string, Animation> _animations;
        Animation _lastAnimation, _currentAnimation;
        int _currentFrameIndex;
        TimeSpan _lastFrameStartTime;
        string _runningAnimationName;
        #endregion

        #region Properties
        public Texture2D Texture { get; private set; }
        public Point FrameSize { get; private set; }
        public int Columns { get; private set; }
        public int Lines { get; private set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects Effect { get; set; }
        public string CurrentAnimation { get; set; }
        #endregion

        #region Constructors
        public Sprite(Texture2D texture, int columns, int lines)
        {
            if (texture.Width % columns != 0)
                throw new ArgumentOutOfRangeException("columns", "Texture size is not multiple of the specified number of columns");
            if (texture.Height % lines != 0)
                throw new ArgumentOutOfRangeException("lines", "Texture size is not multiple of the specified number of lines");
            Texture = texture;
            _animations = new Dictionary<string, Animation>();

            FrameSize = new Point(texture.Width / columns, texture.Height / lines);
            Columns = columns;
            Lines = lines;
        }

        public Sprite(Texture2D texture, Point? frameSize)
        {
            if (frameSize != null)
            {
                if (texture.Width % frameSize.Value.X != 0 ||
                    texture.Height % frameSize.Value.Y != 0)
                    throw new ArgumentOutOfRangeException("frameSize", "Texture size is not multiple of the frame size");
            }

            Texture = texture;
            FrameSize = frameSize ?? new Point(texture.Width, texture.Height);
            Columns = Texture.Width / FrameSize.X;
            Lines = Texture.Height / FrameSize.Y;

            _animations = new Dictionary<string, Animation>();
        }
        #endregion

        #region Public Methods

        #region AddAnimation
        public void AddAnimation(string name, Rectangle[] frameRects, TimeSpan frameDuration, bool repeat = true)
        {
            var frames = frameRects.Select(r => new Frame(Texture, r)).ToArray();
            AddAnimation(name, new Animation(frames, frameDuration, repeat ? 0 : frames.Length - 1));
        }

        public void AddAnimation(string name, int[] frameIndexes, TimeSpan frameDuration, bool repeat = true)
        {
            var frames = frameIndexes.Select(GetFrame).ToArray();
            AddAnimation(name, new Animation(frames, frameDuration, repeat ? 0 : frameIndexes.Length - 1));
        }

        public void AddAnimation(string name, int line, int count, TimeSpan frameDuration, bool repeat = true, int skipFrames = 0)
        {
            if (FrameSize == default(Point))
                throw new InvalidOperationException("No FrameSize was specified");

            var startIndex = (Texture.Width / FrameSize.X) * line + skipFrames;
            var indexes = Enumerable.Range(startIndex, count - skipFrames).ToArray();

            AddAnimation(name, indexes, frameDuration, repeat);
        }

        public void AddAnimation(string name, Animation animation)
        {
            _animations[name] = animation;
        }
        #endregion

        public Frame GetFrame(int index)
        {
            if (FrameSize == default(Point))
                throw new InvalidOperationException("No FrameSize was specified");

            return new Frame
            (
                texture: Texture,
                rectangle: new Rectangle(
                    (index % Columns) * FrameSize.X,
                    (index / Columns) * FrameSize.Y,
                    FrameSize.X,
                    FrameSize.Y)
            );
        }
        #endregion

        #region Game Loop
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, float rotation = 0, Color? color = null, Vector2? scale = null, float layerDepth = 0f)
        {
            var curAnimation = SelectAnimation();
            if (curAnimation == null)
                return;

            var frame = curAnimation.Frames[_currentFrameIndex % curAnimation.Frames.Length];

            if (curAnimation != _lastAnimation)
            {
                _lastFrameStartTime = gameTime.TotalGameTime;
                _lastAnimation = curAnimation;
                _currentFrameIndex = 0;
            }
            else if (gameTime.TotalGameTime > _lastFrameStartTime + curAnimation.FrameDuration)
            {
                _currentFrameIndex++;
                if (_currentFrameIndex >= curAnimation.Frames.Length)
                    _currentFrameIndex = curAnimation.ResetToFrame;
                _lastFrameStartTime = gameTime.TotalGameTime;
            }

            spriteBatch.Draw(frame.Texture, position, null, frame.Rectangle, Origin, rotation, scale, color, Effect, layerDepth);
        }
        #endregion

        #region Private Methods
        Animation SelectAnimation()
        {
            if (_animations.Count == 0)
                return null;

            if (_runningAnimationName != CurrentAnimation || _currentAnimation == null)
            {
                if (CurrentAnimation == null || !_animations.ContainsKey(CurrentAnimation))
                    CurrentAnimation = _animations.First().Key;

                _currentAnimation = _animations[CurrentAnimation];
                _runningAnimationName = CurrentAnimation;
            }
            return _currentAnimation;
        }
        #endregion
    }
}
