using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core.Entities;
using MonoGameLib.Core.Sprites;
using PowerOfLove.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerOfLove
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

        public static Rectangle GetCollisionRectangle(this GamePlayEntity entity, Vector2 translate = default(Vector2))
        {
            var pos = entity.Position + translate;
            var rec = new Rectangle((int)pos.X, (int)pos.Y, (int)(entity.Size.X * entity.Scale.X), (int)(entity.Size.Y * entity.Scale.Y));
            rec.X += entity.CollisionBox.X;
            rec.Width -= entity.CollisionBox.X + entity.CollisionBox.Width;
            rec.Y += entity.CollisionBox.Y;
            rec.Height -= entity.CollisionBox.Y + entity.CollisionBox.Height;
            return rec;
        }

        public static Texture2D AsTrueVision(this Texture2D texture, Game game)
        {
            var oldName = Path.GetFileNameWithoutExtension(texture.Name);
            var oldPath = Path.GetDirectoryName(texture.Name);
            var newName = oldName + "-truevision";
            var newPath = Path.Combine(oldPath, newName);

            return game.Content.Load<Texture2D>(newPath);
        }

        public static Texture2D AsZombie(this Texture2D texture, Game game)
        {
            var oldName = Path.GetFileNameWithoutExtension(texture.Name);
            var oldPath = Path.GetDirectoryName(texture.Name);
            var newName = oldName.Replace("normal", "zombie");
            var newPath = Path.Combine(oldPath, newName);

            return game.Content.Load<Texture2D>(newPath);
        }

        public static TimeSpan Multiply(this TimeSpan time, float by)
        {
            return TimeSpan.FromMilliseconds(time.TotalMilliseconds * by);
        }

        public static TimeSpan Divide(this TimeSpan time, float by)
        {
            return time.Multiply(1 / by);
        }
    }
}
