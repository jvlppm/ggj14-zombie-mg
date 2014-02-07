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

        public GamePlayScreen(Game game)
            : base(game)
        {
            _map = MapLoader.LoadMap("Content/Maps/MainMap.tmx");
            Entities = _map.Objects.Select(CreateEntity).ToList();
        }

        GamePlayEntity CreateEntity(GameObject arg)
        {
            GamePlayEntity newEntity;
            switch(arg.Name)
            {
                case "player":
                    if (_player != null) throw new InvalidOperationException();
                    newEntity = _player = new Player(Game, this);
                    break;
                default:
                    newEntity = new NPC(Game,  this, RandomNumberGenerator.Next(1, 4));
                    break;
            }

            newEntity.Position = arg.Position;
            newEntity.Tag = arg.Category;
            return newEntity;
        }

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch.GraphicsDevice.Clear(MainGame.DefaultBackgroundColor);
            Begin(GetCamera(), SamplerState.PointClamp, rasterize: RasterizerState.CullNone);
            _map.Draw(gameTime, SpriteBatch, Vector2.Zero);

            foreach (var ent in Entities)
                ent.Draw(gameTime, SpriteBatch);

            SpriteBatch.End();
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            foreach (var ent in Entities)
                ent.Update(gameTime);
        }

        public GamePlayEntity NearestToEntity(GamePlayEntity baseEntity, string tag)
        {
            return Entities.Where(s => s != baseEntity && s.Tag == tag)
                           .OrderBy(s => (s.Position - baseEntity.Position).LengthSquared())
                           .FirstOrDefault();
        }

        CameraInfo GetCamera()
        {
            return new CameraInfo(_player.Position, 2);
        }

        void Begin(CameraInfo camera, SamplerState sampler = null, DepthStencilState depthStencil = null, RasterizerState rasterize = null, Effect effect = null)
        {
            var viewPort = Game.GraphicsDevice.Viewport;
            var transformation = Matrix.CreateTranslation(new Vector3(-camera.Position.X, -camera.Position.Y, 0)) *
                //Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(new Vector3(camera.ZoomFactor, camera.ZoomFactor, 1)) *
                Matrix.CreateTranslation(new Vector3(viewPort.Width * 0.5f, viewPort.Height * 0.5f, 0));

            SpriteBatch.Begin(SpriteSortMode.Deferred,
                        BlendState.NonPremultiplied,
                        sampler,
                        depthStencil,
                        rasterize,
                        effect,
                        transformation);
        }
    }
}
