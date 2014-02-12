using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core;
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

        public static Vector2 LimitSize(this Vector2 vector, float maxSize)
        {
            var length = vector.Length();
            if (length > maxSize)
                return vector * (maxSize / length);
            return vector;
        }

        public static void Begin(this SpriteBatch spriteBatch, CameraInfo camera, SamplerState sampler = null, DepthStencilState depthStencil = null, RasterizerState rasterize = null, Effect effect = null)
        {
            var viewPort = spriteBatch.GraphicsDevice.Viewport;
            Vector2 translation = new Vector2(
                (float)Math.Round(-camera.Position.X * camera.ZoomFactor + viewPort.Width * 0.5f),
                (float)Math.Round(-camera.Position.Y * camera.ZoomFactor + viewPort.Height * 0.5f));


            var transformation = Matrix.CreateScale(new Vector3(camera.ZoomFactor, camera.ZoomFactor, 1)) *
                                 Matrix.CreateTranslation(new Vector3(translation, 0));

            spriteBatch.Begin(SpriteSortMode.FrontToBack,
                        BlendState.NonPremultiplied,
                        sampler,
                        depthStencil,
                        rasterize,
                        effect,
                        transformation);
        }

        public static T Random<T>(this IEnumerable<T> items)
        {
            var size = items.Count();
            var index = RandomNumberGenerator.Next(0, size - 1);
            return items.Skip(index).First();
        }
    }
}
