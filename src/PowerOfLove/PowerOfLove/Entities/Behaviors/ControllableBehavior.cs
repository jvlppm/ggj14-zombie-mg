using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
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

            var touchState = TouchPanel.GetState().Cast<TouchLocation?>().FirstOrDefault();
            var mouseState = Mouse.GetState();

            var pointerInfo = new
            {
                Pressed = touchState != null || mouseState.LeftButton == ButtonState.Pressed,
                Location = touchState != null ? touchState.Value.Position : new Vector2(mouseState.X, mouseState.Y)
            };

            var clickPosition = Screen.Camera.ScreenToMapPosition(pointerInfo.Location);
            var direction = clickPosition - Entity.CenterPosition;
            direction.Normalize();
            Entity.Look(direction);

            if (pointerInfo.Pressed)
            {
                if (Entity.Move(direction * (75 * (float)gameTime.ElapsedGameTime.TotalSeconds)))
                    Entity.Sprite.CurrentAnimation = "run";
                else
                    Entity.Sprite.CurrentAnimation = "stand";
            }
            else
                Entity.Sprite.CurrentAnimation = "stand";
        }
    }
}
