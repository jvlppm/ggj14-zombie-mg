using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core.Entities;
using MonoGameLib.Core.Sprites;
using PowerOfLove.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerOfLove.Entities.Behaviors
{
    class GamePlayEntity : Entity
    {
        public GamePlayScreen Screen { get; private set; }

        public Sprite NormalSprite { get; protected set; }
        public Sprite EvilSprite { get; protected set; }

        public bool IsHugging { get; set; }

        public void TurnIntoFriend()
        {
            Tag = "friend";
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

        public bool Move(Microsoft.Xna.Framework.Vector2 direction)
        {
            if(Screen.Move(this, direction))
            {
                Look(direction);
                return true;
            }
            return false;
        }

        public void Look(Vector2 direction)
        {
            if (direction.X < 0)
                Sprite.Effect = SpriteEffects.FlipHorizontally;
            else
                Sprite.Effect = SpriteEffects.None;
        }
    }
}
