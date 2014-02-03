using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.GUI.Base;
using MonoGameLib.GUI.Components;
using MonoGameLib.GUI.Containers;
using PowerOfLove.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Activities
{
    class TitleActivity : Activity<TitleResult>
    {
        List<Component> _uiComponents;

        public TitleActivity(Game game)
            : base(game)
        {
            _uiComponents = new List<Component>
            {
                CreateMainTitle(game),
                CreateMenuOptions(game),
                CreateFooter(game)
            };
        }

        #region GUI
        Component CreateMainTitle(Microsoft.Xna.Framework.Game game)
        {
            return new Label("POWER OF LOVE", "Fonts/BigFont")
            {
                HorizontalOrigin = HorizontalAlign.Center,
                VerticalOrigin = VerticalAlign.Middle,
                Color = Color.Red,
                Position = new Point(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 8)
            };
        }

        Component CreateMenuOptions(Game game)
        {
            var btnNewGame = new Button(game, "New Game");
            btnNewGame.Clicked += (s, e) => Exit(TitleResult.Play);

            var btnHowToPlay = new Button(game, "Help");
            btnHowToPlay.Clicked += (s, e) => Exit(TitleResult.HowToPlay);

            var btnCredits = new Button(game, "Credits");
            btnCredits.Clicked += (s, e) => Exit(TitleResult.Credits);

            var btnExit = new Button(game, "Exit");
            btnExit.Clicked += (s, e) => Exit(TitleResult.Exit);

            var vbox = new VBox
            {
                ItemSpacing = 4,
                Position = new Point(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2),
                VerticalOrigin = VerticalAlign.Middle,
                HorizontalOrigin = HorizontalAlign.Center
            };
            vbox.AddChildren(btnNewGame);
            vbox.AddChildren(btnHowToPlay);
            vbox.AddChildren(btnCredits);
            vbox.AddChildren(btnExit);
            return vbox;
        }

        Component CreateFooter(Microsoft.Xna.Framework.Game game)
        {
            return new Label("Global Game Jam 2014", "Fonts/DefaultFont")
            {
                HorizontalOrigin = HorizontalAlign.Center,
                VerticalOrigin = VerticalAlign.Middle,
                Color = Color.White,
                Position = new Point(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height * 7 / 8)
            };
        }
        #endregion

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(MainGame.DefaultBackgroundColor);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            foreach (var cmp in _uiComponents)
                cmp.Draw(gameTime, SpriteBatch);

            SpriteBatch.End();
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            foreach (var cmp in _uiComponents)
                cmp.Update(gameTime);
        }
    }
}
