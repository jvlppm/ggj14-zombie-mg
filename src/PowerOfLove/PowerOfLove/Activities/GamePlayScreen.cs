using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core;
using MonoGameLib.Tiled;
using PowerOfLove.Entities;
using PowerOfLove.Entities.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Activities
{
    class GamePlayScreen : Activity<int>
    {
        Map _map;

        public IEnumerable<GamePlayEntity> Entities { get; private set; }
        GamePlayEntity _player;
        public bool IsEvil { get; private set; }
        public CameraInfo Camera { get; private set; }

        public GamePlayScreen(Game game)
            : base(game)
        {
            _map = MapLoader.LoadMap("Content/Maps/MainMap.tmx");
            Entities = _map.Objects.Select(CreateEntity).ToList();
        }

        GamePlayEntity CreateEntity(GameObject arg)
        {
            GamePlayEntity newEntity;
            switch (arg.Name)
            {
                case "player":
                    if (_player != null) throw new InvalidOperationException();
                    newEntity = _player = new Player(Game, this);
                    break;
                default:
                    newEntity = new NPC(Game, this, RandomNumberGenerator.Next(1, 4));
                    break;
            }

            newEntity.Position = arg.Position;
            newEntity.Tag = arg.Category;
            return newEntity;
        }

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch.GraphicsDevice.Clear(MainGame.DefaultBackgroundColor);
            Begin(Camera, SamplerState.PointClamp);
            _map.Draw(gameTime, SpriteBatch, Vector2.Zero);

            foreach (var ent in Entities)
                ent.Draw(gameTime, SpriteBatch);

            SpriteBatch.End();
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Camera = new CameraInfo(_player.Position, 2, Game.GraphicsDevice.Viewport);

            foreach (var ent in Entities)
                ent.Update(gameTime);
        }

        public GamePlayEntity NearestToEntity(GamePlayEntity baseEntity, string tag)
        {
            return Entities.Where(s => s != baseEntity && s.Tag == tag)
                           .OrderBy(s => (s.Position - baseEntity.Position).LengthSquared())
                           .FirstOrDefault();
        }

        public bool Move(GamePlayEntity entity, Vector2 direction)
        {
            var possibleDirections = new[]{ direction, new Vector2(direction.X, 0), new Vector2(0, direction.Y)};
            foreach(var dir in possibleDirections)
            {
                if(!_map.Collides(entity.GetCollisionRectangle(dir)))
                {
                    entity.Position += dir;
                    return true;
                }
            }
            return false;
        }

        void Begin(CameraInfo camera, SamplerState sampler = null, DepthStencilState depthStencil = null, RasterizerState rasterize = null, Effect effect = null)
        {
            var viewPort = Game.GraphicsDevice.Viewport;
            Vector2 translation = new Vector2(
                (float)Math.Round(-camera.Position.X * camera.ZoomFactor + viewPort.Width * 0.5f),
                (float)Math.Round(-camera.Position.Y * camera.ZoomFactor + viewPort.Height * 0.5f));


            var transformation = Matrix.CreateScale(new Vector3(camera.ZoomFactor, camera.ZoomFactor, 1)) *
                                 Matrix.CreateTranslation(new Vector3(translation, 0));

            SpriteBatch.Begin(SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        sampler,
                        depthStencil,
                        rasterize,
                        effect,
                        transformation);
        }
    }
}
