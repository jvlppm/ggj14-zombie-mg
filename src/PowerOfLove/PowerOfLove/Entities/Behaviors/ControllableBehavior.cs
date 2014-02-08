using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Entities;

namespace PowerOfLove.Entities.Behaviors
{
    class TouchControlBehavior : GameEntityBehavior
    {
        public TouchControlBehavior(GamePlayEntity parent)
            : base(parent)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (Entity.IsHugging)
                return;

            var mState = Mouse.GetState();
            var clickPosition = Screen.Camera.ScreenToMapPosition(mState.Position);
            var direction = clickPosition - Entity.Position;
            direction.Normalize();
            Entity.Look(direction);

            if(mState.LeftButton == ButtonState.Pressed)
            {
                if (Entity.Move(direction))
                    Entity.Sprite.CurrentAnimation = "run";
                else
                    Entity.Sprite.CurrentAnimation = "stand";
            }
            else
                Entity.Sprite.CurrentAnimation = "stand";
        }
    }
}
