using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGameLib.Core.Sprites;
using MonoGameLib.GUI.Base;
using MonoGameLib.GUI.Components;
using PowerOfLove.Components;
using System;

namespace PowerOfLove.Activities
{
    class LeaderboardsScreen : Activity
    {
        #region Attributes
        GUI _gui;
        float textBasePosY;
        float textPosY;
        Song _music;
        #endregion

        #region Constructors
        public LeaderboardsScreen(Game game)
            : base(game)
        {
            _gui = new GUI(new Vector2(GraphicsDevice.Viewport.Height / 500f));
            textBasePosY = 40 * _gui.Scale.Y;
            textPosY = textBasePosY;

            CreateHeaders();
            AddPlayerScore("Joao Vitor P. Moraes", "Android", 10, 90);
            AddPlayerScore("Diogo Muller", "Facebook", 10, 90);

            CreateBackButton(game);

            _music = Game.Content.Load<Song>("Audio/Music/credits.wav");
        }
        #endregion

        #region GUI
        void CreateHeaders()
        {
            var playerNameLabel = new Label("NAME", "Fonts/DefaultFont")
            {
                Color = Color.Red,
                Position = new Point(Viewport.Width / 8, (int)textPosY)
            };
            _gui.Add(playerNameLabel);

            _gui.Add(new Label("HIGH SCORE", "Fonts/DefaultFont")
            {
                Color = Color.Red,
                HorizontalOrigin = HorizontalAlign.Right,
                Position = new Point((int)(Viewport.Width * 5 / 8.0f), (int)textPosY)
            });

            _gui.Add(new Label("TOTAL ZOMBIES", "Fonts/DefaultFont")
            {
                Color = Color.Red,
                HorizontalOrigin = HorizontalAlign.Right,
                Position = new Point((int)(Viewport.Width * 6 / 8.0f + 80 * _gui.Scale.X), (int)textPosY)
            });

            textPosY += playerNameLabel.MeasureSize().Y;
        }

        void AddPlayerScore(string name, string map, int highScore, int totalZombies)
        {
            var playerNameLabel = new Label(name, "Fonts/DefaultFont")
            {
                Color = Color.YellowGreen,
                Position = new Point(Viewport.Width / 8, (int)textPosY)
            };
            _gui.Add(playerNameLabel);

            _gui.Add(new Label(highScore.ToString(), "Fonts/DefaultFont")
            {
                Color = Color.White,
                HorizontalOrigin = HorizontalAlign.Right,
                Position = new Point((int)(Viewport.Width * 5 / 8.0f), (int)textPosY)
            });

            _gui.Add(new Label(highScore.ToString(), "Fonts/DefaultFont")
            {
                Color = Color.White,
                HorizontalOrigin = HorizontalAlign.Right,
                Position = new Point((int)(Viewport.Width * 6 / 8.0f + 80 * _gui.Scale.X), (int)textPosY)
            });

            textPosY += playerNameLabel.MeasureSize().Y;
        }

        void CreateBackButton(Game game)
        {
            var btnBack = new Button(game, "Return") { HorizontalOrigin = HorizontalAlign.Center };
            btnBack.Position = new Point(
                Viewport.Width / 2,
                Viewport.Height * 7 / 8);
            btnBack.Clicked += (s, e) => Exit();
            _gui.Add(btnBack);
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
