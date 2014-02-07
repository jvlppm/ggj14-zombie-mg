using MonoGameLib.Core.Entities;
using MonoGameLib.Core.Sprites;
using PowerOfLove.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Entities.Behaviors
{
    class GamePlayEntity : Entity
    {
        public GamePlayScreen Screen { get; private set; }

        public Sprite EvilSprite { get; protected set; }

        public void TurnEvil()
        {
            Sprite = EvilSprite;
        }

        public GamePlayEntity(GamePlayScreen screen)
        {
            if (screen == null)
                throw new ArgumentNullException("screen");
            Screen = screen;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.Color? colorOverride = null, Microsoft.Xna.Framework.Vector2? scaleOverride = null)
        {
            base.Draw(gameTime, spriteBatch, colorOverride, scaleOverride);
        }
    }
}
