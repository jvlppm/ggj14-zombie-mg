using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGameLib.GUI.Base;
using MonoGameLib.GUI.Components;
using System;
using System.Linq;

namespace PowerOfLove.Components
{
    class Button : Label
    {
        Texture2D _normal, _over, _pressed;

        public event EventHandler Clicked;

        public Button(Game game, string text)
            : base(text, "Fonts/DefaultFont")
        {
            _normal =  game.Content.Load<Texture2D>("Images/GUI/ButtonNormal");
            _over = game.Content.Load<Texture2D>("Images/GUI/ButtonOver");
            _pressed = game.Content.Load<Texture2D>("Images/GUI/ButtonPressed");
            Color = Microsoft.Xna.Framework.Color.White;
            Size = new Microsoft.Xna.Framework.Point(_normal.Width, _normal.Height);
            HorizontalOrigin = MonoGameLib.GUI.Base.HorizontalAlign.Center;
            VerticalOrigin = MonoGameLib.GUI.Base.VerticalAlign.Middle;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            var originalPosition = Position;

            var origin = AlignExtensions.ToVector(HorizontalOrigin, VerticalOrigin);
            Position = new Point((int)(Position.X - (Size.X * origin.X)), (int)(Position.Y - (Size.Y * origin.Y)));

            var position = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
            

            var touchState = TouchPanel.GetState().Cast<TouchLocation?>().FirstOrDefault();
            var mouseState = Mouse.GetState();

            var pointerInfo = new
            {
                Pressed = touchState != null || mouseState.LeftButton == ButtonState.Pressed,
                Location = touchState != null? touchState.Value.Position : new Vector2(mouseState.X, mouseState.Y)
            };

            if (!position.Contains(pointerInfo.Location))
                spriteBatch.Draw(_normal, position, null, Microsoft.Xna.Framework.Color.White, 0, Scale, SpriteEffects.None, 0);
            else if (!pointerInfo.Pressed)
                spriteBatch.Draw(_over, position, null, Microsoft.Xna.Framework.Color.White, 0, Scale, SpriteEffects.None, 0);
            else
            {
                spriteBatch.Draw(_pressed, position, null, Microsoft.Xna.Framework.Color.White, 0, Scale, SpriteEffects.None, 0);
                if (Clicked != null)
                    Clicked(this, EventArgs.Empty);
            }

            base.Draw(gameTime, spriteBatch);
            Position = originalPosition;
        }
    }
}
