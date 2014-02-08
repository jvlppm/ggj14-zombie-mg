using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core;
using MonoGameLib.GUI.Components;
using MonoGameLib.Tiled;
using PowerOfLove.Components;
using PowerOfLove.Entities;
using PowerOfLove.Entities.Behaviors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerOfLove.Activities
{
    class GamePlayScreen : Activity<int>
    {
        Map _map;
        Label _gameTimerLabel;

        public ContextTimer Timer { get; private set; }

        public IEnumerable<GamePlayEntity> Entities { get; private set; }
        public GamePlayEntity Player { get; private set; }
        public bool IsTrueVision { get; private set; }
        public CameraInfo Camera { get; private set; }

        public GamePlayScreen(Game game)
            : base(game)
        {
            _gameTimerLabel = new Label("", "Fonts/BigFont") { Color = Color.Red, Position = new Point(16,0) };

            _map = MapLoader.LoadMap("Content/Maps/MainMap.tmx");
            Entities = _map.Objects.Select(CreateEntity).ToList();
            Timer = new ContextTimer(TimeSpan.FromMinutes(1));
        }

        GamePlayEntity CreateEntity(GameObject arg)
        {
            GamePlayEntity newEntity;
            switch (arg.Name)
            {
                case "player":
                    if (Player != null) throw new InvalidOperationException();
                    newEntity = Player = new Player(Game, this);
                    break;
                default:
                    newEntity = new NPC(Game, this, RandomNumberGenerator.Next(1, 4));
                    break;
            }

            newEntity.LayerDepth = 0.5f;
            newEntity.Position = arg.Position;
            newEntity.Tag = arg.Name;
            return newEntity;
        }

        #region Activity Life Cycle
        protected async override Task<int> RunActivity()
        {
            var finishTimer = UpdateContext.Run(Timer);
            await UpdateContext.Delay(TimeSpan.FromSeconds(30));
            ShowTrueVision();
            await finishTimer;
            return Entities.Count(e => e.Tag == "friend");
        }
        #endregion

        #region Game Loop
        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Camera = new CameraInfo(Player.CenterPosition, 2, Game.GraphicsDevice.Viewport);

            foreach (var ent in Entities)
                ent.Update(gameTime);
        }

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch.GraphicsDevice.Clear(MainGame.DefaultBackgroundColor);
            Begin(Camera, SamplerState.PointClamp);

            _map.Draw(gameTime, SpriteBatch, Vector2.Zero);

            foreach (var ent in Entities.OrderBy(e => e.Position.Y))
            {
                ent.LayerDepth = 0.5f + (ent.Position.Y / _map.PixelSize.Y / 100);
                ent.Draw(gameTime, SpriteBatch);
            }

            SpriteBatch.End();

            SpriteBatch.Begin();
            _gameTimerLabel.Text = Timer.RemainingTime.TotalSeconds.ToString("00");
            _gameTimerLabel.Draw(gameTime, SpriteBatch);
            SpriteBatch.End();
        }

        void Begin(CameraInfo camera, SamplerState sampler = null, DepthStencilState depthStencil = null, RasterizerState rasterize = null, Effect effect = null)
        {
            var viewPort = Game.GraphicsDevice.Viewport;
            Vector2 translation = new Vector2(
                (float)Math.Round(-camera.Position.X * camera.ZoomFactor + viewPort.Width * 0.5f),
                (float)Math.Round(-camera.Position.Y * camera.ZoomFactor + viewPort.Height * 0.5f));


            var transformation = Matrix.CreateScale(new Vector3(camera.ZoomFactor, camera.ZoomFactor, 1)) *
                                 Matrix.CreateTranslation(new Vector3(translation, 0));

            SpriteBatch.Begin(SpriteSortMode.FrontToBack,
                        BlendState.NonPremultiplied,
                        sampler,
                        depthStencil,
                        rasterize,
                        effect,
                        transformation);
        }
        #endregion

        #region Public Methods
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

        public void ShowTrueVision()
        {
            foreach (var tileset in _map.Tilesets)
                tileset.Texture = tileset.Texture.AsTrueVision(Game);
            IsTrueVision = true;
        }
        #endregion
    }
}
