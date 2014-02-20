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
            AddPlayerScore("Joao Vitor P. Moraes", "Android", 10, 90);
            AddPlayerScore("Diogo Muller", "Facebook", 10, 90);

            CreateBackButton(game);

            _music = Game.Content.Load<Song>("Audio/Music/credits.wav");
        }
        #endregion

        #region GUI
        void AddPlayerScore(string name, string map, int highScore, int totalZombies)
        {

            var textPosX = Game.GraphicsDevice.Viewport.Width / 2 - 80 * _gui.Scale.X;
            var cat1Title = new Label(name, "Fonts/DefaultFont")
            {
                //FontSize = 24,
                Color = Color.YellowGreen,
                Position = new Point((int)textPosX, (int)textPosY)
            };

            _gui.Add(cat1Title);

            textPosY += cat1Title.MeasureSize().Y;

            if (textPosX == 0)
                textPosX = cat1Title.Position.X;

            var cat1Text = new Label(map, "Fonts/DefaultFont")
            {
                Color = Color.White,
                Position = new Point((int)textPosX, (int)textPosY)
            };

            _gui.Add(cat1Text);
            textPosY += cat1Text.MeasureSize().Y;
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
