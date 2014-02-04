using Microsoft.Xna.Framework;
using MonoGameLib.GUI.Base;
using PowerOfLove.Base;

namespace PowerOfLove.Components
{
    class SpriteComponent : Component
    {
        public Sprite Sprite { get; private set; }
        public Vector2 Scale { get; set; }

        public SpriteComponent(Sprite sprite)
        {
            Sprite = sprite;
            Scale = Vector2.One;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            var position = new Vector2(Position.X, Position.Y);
            var origin = AlignExtensions.ToVector(HorizontalOrigin, VerticalOrigin);
            var size = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y) * Scale;
            Sprite.Draw(gameTime, spriteBatch, position - size * origin, scale: Scale);
        }
    }
}
