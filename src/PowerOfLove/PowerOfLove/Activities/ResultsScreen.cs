﻿using Jv.Games.Xna.Async;
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
    class ResultsScreen : Activity
    {
        GUI _gui;

        public ResultsScreen(Game game, int gamePlayResult)
            : base(game)
        {
            _gui = new GUI
            {
                CreateMessage(gamePlayResult),
                CreateBackButton(game)
            };
        }

        Component CreateMessage(int gameResult)
        {
            return new Label("Sorry if we, from Curitiba, look antisocial some times.\n" +
                             "We're just afraid to be turned into zombies.\n" +
                             "\n" +
                             "You zombified " + gameResult + " people.", "Fonts/DefaultFont")
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

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _gui.Draw(gameTime, SpriteBatch);
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _gui.Update(gameTime);
        }
    }
}