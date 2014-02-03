using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.GUI.Base;
using MonoGameLib.GUI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Components
{
    class Button : Label
    {
        Texture2D _normal, _over, _pressed;

        public event EventHandler Clicked;

        public Button(Game game, string text)
            : base(text, "Fonts/DefaultFont")
        {

            _normal = game.Content.Load<Texture2D>("Images/GUI/ButtonNormal.png");
            _over = game.Content.Load<Texture2D>("Images/GUI/ButtonOver.png");
            _pressed = game.Content.Load<Texture2D>("Images/GUI/ButtonPressed.png");
            Color = Microsoft.Xna.Framework.Color.White;
            Size = new Microsoft.Xna.Framework.Point(_normal.Width, _normal.Height);
            HorizontalOrigin = MonoGameLib.GUI.Base.HorizontalAlign.Center;
            VerticalOrigin = MonoGameLib.GUI.Base.VerticalAlign.Middle;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            var position = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);

            var mouseState = Mouse.GetState();
            if (!position.Contains(new Point(mouseState.X, mouseState.Y)))
                spriteBatch.Draw(_normal, position, Microsoft.Xna.Framework.Color.White);
            else if (mouseState.LeftButton == ButtonState.Released)
                spriteBatch.Draw(_over, position, Microsoft.Xna.Framework.Color.White);
            else
            {
                spriteBatch.Draw(_pressed, position, Microsoft.Xna.Framework.Color.White);
                if (Clicked != null)
                    Clicked(this, EventArgs.Empty);
            }

            base.Draw(gameTime, spriteBatch);
        }
    }
}
