using Microsoft.Xna.Framework;
using MonoGameLib.Core.Sprites;
using MonoGameLib.GUI.Base;

namespace PowerOfLove.Components
{
    class SpriteComponent : Component
    {
        public Sprite Sprite { get; private set; }

        public SpriteComponent(Sprite sprite)
        {
            Sprite = sprite;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            var position = new Vector2(Position.X, Position.Y);
            var origin = AlignExtensions.ToVector(HorizontalOrigin, VerticalOrigin);
            var size = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y) * Scale;
            Sprite.Update(gameTime);
            Sprite.Draw(gameTime, spriteBatch, position - size * origin, scale: Scale);
        }
    }
}
