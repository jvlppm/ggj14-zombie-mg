using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGameLib.GUI.Base;
using MonoGameLib.GUI.Components;
using PowerOfLove.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if ANDROID
using PowerOfLove.Helpers;
#endif

namespace PowerOfLove.Activities
{
    class ResultsScreen : Activity
    {
        #region Attributes
        GUI _gui;
        Song _music;
        #endregion

        #region Constructors
        public ResultsScreen(Game game, int gamePlayResult)
            : base(game)
        {
            _gui = new GUI(new Vector2(GraphicsDevice.Viewport.Height / 500f))
            {
                CreateMainTitle(game),
                CreateMessage(gamePlayResult),
                CreateBackButton(game)
            };
            _music = Game.Content.Load<Song>("Audio/Music/credits.wav");

#if ANDROID
            PowerOfLoveServer.Instance.PostResultToServerAsync(gamePlayResult);
#endif
        }
        #endregion

        #region GUI
        Component CreateMainTitle(Game game)
        {
            return new Label("THE END", "Fonts/BigFont")
            {
                HorizontalOrigin = HorizontalAlign.Center,
                VerticalOrigin = VerticalAlign.Middle,
                Color = Color.White,
                Position = new Point(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 8)
            };
        }

        Component CreateMessage(int gameResult)
        {
            return new Label("You zombified " + gameResult + " people.", "Fonts/DefaultFont")
            {
                Color = Color.White,
                HorizontalOrigin = HorizontalAlign.Center,
                VerticalOrigin = VerticalAlign.Middle,
                Position = new Point(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2 - 60)
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

        #region Activity Life-Cycle
        protected override void Activating()
        {
            MediaPlayer.Play(_music);
            base.Activating();
        }

        protected override void Deactivating()
        {
            MediaPlayer.Stop();
            base.Deactivating();
        }
        #endregion

        #region Game Loop
        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch.GraphicsDevice.Clear(MainGame.DefaultBackgroundColor);
            _gui.Draw(gameTime, SpriteBatch);
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            _gui.Update(gameTime);
        }
        #endregion
    }
}
