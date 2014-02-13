using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGameLib.Core;
using MonoGameLib.GUI.Base;
using MonoGameLib.GUI.Components;
using MonoGameLib.GUI.Containers;
using PowerOfLove.Components;

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
            Exit
        }
        #endregion

        #region Attributes
        GUI _gui;
        SoundEffectInstance _bgmInstance;
        #endregion

        #region Constructors
        public TitleScreen(Game game)
            : base(game)
        {
            _gui = new GUI
            {
                CreateMainTitle(game),
                CreateMenuOptions(game),
                CreateFooter(game)
            };

            SoundEffect music;
            if (RandomNumberGenerator.Next(0, 9) != 9)
                music = Game.Content.Load<SoundEffect>("Audio/Music/title.wav");
            else
                music = Game.Content.Load<SoundEffect>("Audio/Music/title_reversed.wav");

            _bgmInstance = music.CreateInstance();
            _bgmInstance.IsLooped = true;
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

        #region Activity Life-Cycle
        protected override void Activating()
        {
            _bgmInstance.Play();
            base.Activating();
        }

        protected override void Deactivating()
        {
            _bgmInstance.Pause();
            base.Deactivating();
        }
        #endregion

        #region Game Loop
        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _gui.Draw(gameTime, SpriteBatch);
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _gui.Update(gameTime);
        }
        #endregion
    }
}
