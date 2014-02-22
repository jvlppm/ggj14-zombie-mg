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
using MonoGameLib.GUI.Containers;
#endif

namespace PowerOfLove.Activities
{
    class ResultsScreen : Activity
    {
        #region Attributes
        GUI _gui;
        VBox _messages;
        Song _music;
        #endregion

        #region Constructors
        public ResultsScreen(Game game, int gamePlayResult)
            : base(game)
        {
            _gui = new GUI(new Vector2(GraphicsDevice.Viewport.Height / 500f));
            _gui.Add(CreateMainTitle(game));
            _gui.Add(CreateMessage(gamePlayResult));
            _gui.Add(CreateBackButton(game));

            _music = Game.Content.Load<Song>("Audio/Music/credits.wav");

#if ANDROID && !DEBUG
            string facebookId = Facebook.Instance.UserId;
            if (facebookId != null)
                PowerOfLoveService.Instance.PostResultToServerAsync(facebookId, gamePlayResult);
#endif

            LoadServerData();
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
            _messages = new VBox
            {
                ItemSpacing = (int)(56 * _gui.Scale.Y),
                Position = new Point(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 3),
                VerticalOrigin = VerticalAlign.Top,
                HorizontalOrigin = HorizontalAlign.Center
            };

            _messages.AddChildren(new Label("You zombified " + gameResult + " people.", "Fonts/DefaultFont")
            {
                Color = Color.White,
                HorizontalOrigin = HorizontalAlign.Center,
            });

            return _messages;
        }

        Component CreateBackButton(Microsoft.Xna.Framework.Game game)
        {
            var btnBack = new Button(game, "Return") { HorizontalOrigin = HorizontalAlign.Center };
            btnBack.Position = new Point(
                Game.GraphicsDevice.Viewport.Width / 2,
                Game.GraphicsDevice.Viewport.Height * 7 / 8);
            btnBack.Clicked += (s, e) => Exit();
            return btnBack;
        }

        async void LoadServerData()
        {
            try
            {
                var facebookId = Facebook.Instance.UserId;
                if (facebookId == null)
                    return;

                var userInfo = await PowerOfLoveService.Instance.GetUserInfoAsync(facebookId, true);

                _messages.AddChildren(new Label("Your highscore: " + userInfo.HighScore, "Fonts/DefaultFont")
                {
                    Scale = _gui.Scale,
                    Color = Color.White,
                    HorizontalOrigin = HorizontalAlign.Center,
                });

                _messages.AddChildren(new Label("You have turned " + userInfo.TotalZombies + " people into zombies on total.", "Fonts/DefaultFont")
                {
                    Scale = _gui.Scale,
                    Color = Color.White,
                    HorizontalOrigin = HorizontalAlign.Center,
                });
            }
            catch (Exception) { }
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
