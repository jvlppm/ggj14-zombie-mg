using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Entities.Behaviors
{
    class ControllableBehavior : Behavior
    {
        new GamePlayEntity Entity { get { return (GamePlayEntity) base.Entity; } }

        public ControllableBehavior(GamePlayEntity parent)
            : base(parent)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Entity.IsHugging)
                return;

            var mState = Mouse.GetState();
            if(mState.LeftButton == ButtonState.Pressed)
            {
                var camera = Entity.Screen.Camera;
                var clickPosition = camera.ScreenToMapPosition(mState.Position);
                var direction = clickPosition - Entity.Position;
                direction.Normalize();

                if (Entity.Move(direction))
                    Entity.Sprite.CurrentAnimation = "run";
                else
                    Entity.Sprite.CurrentAnimation = "stand";

                if (Entity.Position.X > clickPosition.X)
                    Entity.Sprite.Effect = SpriteEffects.FlipHorizontally;
                else
                    Entity.Sprite.Effect = SpriteEffects.None;
            }
            else
                Entity.Sprite.CurrentAnimation = "stand";
        }
    }
}
