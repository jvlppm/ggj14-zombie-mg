using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.GUI.Base;
using MonoGameLib.GUI.Components;
using PowerOfLove.Components;
using System.Collections.Generic;

namespace PowerOfLove.Activities
{
    class HowToPlayActivity : Activity
    {
        GUI _gui;

        public HowToPlayActivity(Game game)
            : base(game)
        {
            _gui = new GUI
            {
                CreateObjective(game),
                CreateKeyBindings(game),
                CreateBackButton(game)
            };
        }

        #region GUI
        Component CreateObjective(Game game)
        {
            return new Label("Objective: You have to save as many people as you can.\r\n           Hug them to save'em with your love.", "Fonts/DefaultFont")
            {
                Color = Color.White,
                HorizontalOrigin = HorizontalAlign.Center,
                VerticalOrigin = VerticalAlign.Middle,
                Position = new Point(game.GraphicsDevice.Viewport.Width / 2, (int)(game.GraphicsDevice.Viewport.Height / 4 * 1.7f - 60))
            };
        }
        Component CreateKeyBindings(Game game)
        {
            return new Label("Movement: Keyboard Arrows or WASD\r\n", "Fonts/DefaultFont")
            {
                Color = Color.White,
                HorizontalOrigin = HorizontalAlign.Center,
                VerticalOrigin = VerticalAlign.Middle,
                Position = new Point(game.GraphicsDevice.Viewport.Width / 2, (int)(game.GraphicsDevice.Viewport.Height / 4 * 2.7f - 60))
            };
        }
        Component CreateBackButton(Microsoft.Xna.Framework.Game game)
        {
            var btnBack = new Button(game, "Return") { HorizontalOrigin = HorizontalAlign.Center };
            btnBack.Position = new Point(
                Game.GraphicsDevice.Viewport.Width / 2 - btnBack.Size.X / 2,
                Game.GraphicsDevice.Viewport.Height * 7 / 8);
            btnBack.Clicked += (s, e) => Exit();
            return btnBack;
        }
        #endregion

        protected override void Draw(GameTime gameTime)
        {
            _gui.Draw(gameTime, SpriteBatch);
        }

        protected override void Update(GameTime gameTime)
        {
            _gui.Update(gameTime);
        }
    }
}
