﻿using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGameLib.Core;
using MonoGameLib.GUI.Base;
using MonoGameLib.GUI.Components;
using MonoGameLib.GUI.Containers;
using PowerOfLove.Components;
#if ANDROID
using Xamarin.Social.Services;
using Xamarin.Auth;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Net.Http;
using MonoGameLib.Core.Sprites;
using Conversive.PHPSerializationLibrary;
using System.Collections;
using System.Text.RegularExpressions;
using PowerOfLove.Helpers;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace PowerOfLove.Activities
{
    class TitleScreen : Activity<TitleScreen.Result>
    {
        #region Nested
        public enum Result
        {
            Credits,
            HowToPlay,
            Play,
            Exit,
            Leaderboards
        }
        #endregion

        #region Attributes
        GUI _gui;
        Song _music;
#if ANDROID
        private Label _lblWelcome;
        private Label _lblHighscore;
        private Label _lblTotalZombies;
        private Container _facebookStatus;
        private Button _facebookLogin;
#endif
        #endregion

        #region Constructors
        public TitleScreen(Game game)
            : base(game)
        {
            _gui = new GUI(new Vector2(GraphicsDevice.Viewport.Height / 500f))
            {
                CreateMainTitle(game),
                CreateFooter(game)
            };
            _gui.Add(CreateMenuOptions(game));
#if ANDROID
            CreateFacebookLogin(game);
            CreateFacebookStatus(game);
            RefreshStatusAsync(game);
#endif

            if (RandomNumberGenerator.Next(0, 9) != 9)
                _music = Game.Content.Load<Song>("Audio/Music/title.wav");
            else
                _music = Game.Content.Load<Song>("Audio/Music/title_reversed.wav");
        }
        #endregion

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
            btnNewGame.Clicked += (s, e) => Exit(Result.Play);

            var btnHowToPlay = new Button(game, "Help");
            btnHowToPlay.Clicked += (s, e) => Exit(Result.HowToPlay);

            var btnCredits = new Button(game, "Credits");
            btnCredits.Clicked += (s, e) => Exit(Result.Credits);

            var btnExit = new Button(game, "Exit");
            btnExit.Clicked += (s, e) => Exit(Result.Exit);

            var vbox = new VBox
            {
                ItemSpacing = (int)(16 * _gui.Scale.Y),
#if ANDROID
                Position = new Point(Viewport.Width / 4, Viewport.Height / 3),
                VerticalOrigin = VerticalAlign.Top,
#else
                Position = new Point(Viewport.Width / 2, Viewport.Height / 2),
                VerticalOrigin = VerticalAlign.Middle,
#endif
                HorizontalOrigin = HorizontalAlign.Center
            };
            vbox.AddChildren(btnNewGame);
            vbox.AddChildren(btnHowToPlay);
            vbox.AddChildren(btnCredits);

            vbox.AddChildren(btnExit);
            return vbox;
        }

#if ANDROID
        void CreateFacebookLogin(Game game)
        {
            _facebookLogin = new Button("", game.Content.Load<Texture2D>("Images/GUI/FacebookLogin"), null, null)
            {
                Position = new Point(Viewport.Width * 3 / 4, Viewport.Height / 2),
                VerticalOrigin = VerticalAlign.Middle,
                HorizontalOrigin = HorizontalAlign.Center
            };

            _facebookLogin.Clicked += facebookLogin_Clicked;
            _gui.Add(_facebookLogin);
        }

        void CreateFacebookStatus(Game game)
        {
            _facebookStatus = new Container();

            var basePosition = new Point((int)(Viewport.Width * 1.5f / 4), Viewport.Height / 3);

            var textureData = new Microsoft.Xna.Framework.Graphics.Texture2D(GraphicsDevice, 20, 20);

            var guiScale = new Point((int)_gui.Scale.X, (int)_gui.Scale.Y);
            
            _lblWelcome = new Label(string.Format("Welcome, {0}", "<username>"), "Fonts/DefaultFont")
            {
                HorizontalOrigin = HorizontalAlign.Left,
                VerticalOrigin = VerticalAlign.Top,
                Color = Color.White,
                Position = basePosition + new Point(48, 0) * guiScale
            };
            _lblHighscore = new Label(string.Format("Your highest score is: {0} zombies.", 0), "Fonts/DefaultFont")
            {
                HorizontalOrigin = HorizontalAlign.Left,
                VerticalOrigin = VerticalAlign.Top,
                Color = Color.White,
                Position = basePosition + new Point(48, 48) * guiScale
            };
            _lblTotalZombies = new Label(string.Format("On total, you saved {0} zombies.", 0), "Fonts/DefaultFont")
            {
                HorizontalOrigin = HorizontalAlign.Left,
                VerticalOrigin = VerticalAlign.Top,
                Color = Color.White,
                Position = basePosition + new Point(48, 48 * 2) * guiScale
            };

            var btnLeaderboards = new Button(game, "Scores")
            {
                HorizontalOrigin = HorizontalAlign.Left,
                VerticalOrigin = VerticalAlign.Top,
                Position = basePosition + new Point(48, 154) * guiScale
            };
            btnLeaderboards.Clicked += (s, e) => Exit(Result.Leaderboards);
            _facebookStatus.AddChildren(btnLeaderboards);

            _facebookStatus.AddChildren(_lblWelcome);
            _facebookStatus.AddChildren(_lblHighscore);
            _facebookStatus.AddChildren(_lblTotalZombies);

            _gui.Add(_facebookStatus);
        }
#endif

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
                Exit(Result.Exit);

            _gui.Update(gameTime);
        }
        #endregion

        #region Facebook
#if ANDROID
        async void facebookLogin_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                _facebookLogin.IsVisible = false;
                await Facebook.Instance.LogInAsync(Game.Activity).On(UpdateContext);
                RefreshStatusAsync(Game);
            }
            catch (Exception)
            {
                _facebookLogin.IsVisible = true;
                _facebookStatus.IsVisible = false;
            }
        }

        async void RefreshStatusAsync(Game game)
        {
            try
            {
                _facebookLogin.IsVisible = false;
                _facebookStatus.IsVisible = false;

                bool isLoggedIn = await Facebook.Instance.IsLoggedInAsync(Game.Activity).On(UpdateContext);

                _facebookLogin.IsVisible = !isLoggedIn;
                _facebookStatus.IsVisible = isLoggedIn;

                if (!isLoggedIn)
                    return;

                _lblWelcome.Text = "Loading Facebook data";
                _lblHighscore.Text = _lblTotalZombies.Text = string.Empty;

                await Facebook.Instance.RefreshUserStatus(Game.Activity).On(UpdateContext);

                Facebook.Instance.LoadFriendNamesAsync();

                _lblWelcome.Text = string.Format("Welcome, {0}", Facebook.Instance.UserName);
                _lblHighscore.Text = "Loading";
                _lblTotalZombies.Text = string.Empty;
                _facebookStatus.IsVisible = true;

                var userStatus = await PowerOfLoveService.Instance.GetUserInfoAsync(Facebook.Instance.UserId, true).On(UpdateContext);

                _lblHighscore.Text = string.Format("Your highest score is: {0} zombies.", userStatus.HighScore);
                _lblTotalZombies.Text = string.Format("On total, you saved {0} zombies.", userStatus.TotalZombies);
            }
            catch (Exception)
            {
                _facebookLogin.IsVisible = true;
                _facebookStatus.IsVisible = false;
            }
        }
#endif
        #endregion
    }
}
