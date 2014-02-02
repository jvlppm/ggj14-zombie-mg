using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using MonoGameLib.GUI.Base;
using MonoGameLib.GUI.Components;
using PowerOfLove.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Activities
{
    class CreditsActivity : Activity
    {
        List<Component> _uiComponents;
        private float textBasePosY = 50;
        private float textBasePosX = 0;

        public CreditsActivity(Game game)
            : base(game)
        {
            _uiComponents = new List<Component>();
            CreateCategory("Game Design", "Diogo Muller de Miranda\r\nRicardo Takeda");
            CreateCategory("Developers ", "Diogo Muller de Miranda\r\nJoao Vitor Pietsiaki Moraes\r\nErik Onuki");
            CreateCategory("Game Art", "Diogo Muller de Miranda\r\nRicardo Takeda");
            CreateCategory("Music and Sound Effects", "Ricardo Takeda");

            var btnBack = new Button(game, "Return")
            {
                HorizontalOrigin = HorizontalAlign.Center,
                Position = new Point(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height * 7 / 8)
            };
            btnBack.Position = new Point(btnBack.Position.X - btnBack.Size.X / 2, btnBack.Position.Y);
            btnBack.Clicked += (s, e) => Exit();
            _uiComponents.Add(btnBack);
        }

        #region GUI
        void CreateCategory(string title, string text)
        {
            if (textBasePosX == 0)
                textBasePosX = Game.GraphicsDevice.Viewport.Width / 2 - 80;

            var cat1Title = new Label(title, "Fonts/DefaultFont")
            {
                //FontSize = 24,
                Color = Color.YellowGreen,
                Position = new Point((int)textBasePosX, (int)textBasePosY)
            };

            _uiComponents.Add(cat1Title);

            textBasePosY += cat1Title.MeasureSize().Y;

            if (textBasePosX == 0)
                textBasePosX = cat1Title.Position.X;

            var cat1Text = new Label(text, "Fonts/DefaultFont")
            {
                Color = Color.White,
                Position = new Point((int)textBasePosX, (int)textBasePosY)
            };

            _uiComponents.Add(cat1Text);
            textBasePosY += cat1Text.MeasureSize().Y + 20;
        }
        #endregion

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin();

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
