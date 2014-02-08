using Microsoft.Xna.Framework;
using MonoGameLib.Core.Entities;
using MonoGameLib.Core.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerOfLove.Entities
{
    static class Extensions
    {
        public static Task PlayAnimation(this Sprite sprite, string animationName)
        {
            var tcs = new TaskCompletionSource<bool>();
            EventHandler animationCompletedHandler = null;
            animationCompletedHandler = delegate
            {
                tcs.TrySetResult(true);
                sprite.AnimationCompleted -= animationCompletedHandler;
            };
            sprite.AnimationCompleted += animationCompletedHandler;
            sprite.StartAnimation(animationName);
            return tcs.Task;
        }

        public static Rectangle GetCollisionRectangle(this Entity entity, Vector2 translate)
        {
            var pos = entity.Position + translate;
            return new Rectangle((int)pos.X, (int)pos.Y, (int)(entity.Size.X * entity.Scale.X), (int)(entity.Size.Y * entity.Scale.Y));
        }
    }
}
