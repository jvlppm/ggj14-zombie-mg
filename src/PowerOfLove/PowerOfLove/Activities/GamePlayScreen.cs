using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core;
using MonoGameLib.GUI.Components;
using MonoGameLib.Tiled;
using PowerOfLove.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerOfLove.Activities
{
    class GamePlayScreen : Activity<int>
    {
        #region Attributes
        Map _map;
        Label _gameTimerLabel, _visionLabel;
        List<GamePlayEntity> _newEntities, _oldEntities;
        SoundEffectInstance _bgmInstance;
        #endregion

        #region Properties
        public ContextTimer Timer { get; private set; }
        public int Score { get; set; }

        public List<GamePlayEntity> Entities { get; private set; }
        public GamePlayEntity Player { get; private set; }
        public bool IsTrueVision { get; private set; }
        public CameraInfo Camera { get; private set; }
        #endregion

        #region Constructors
        public GamePlayScreen(Game game)
            : base(game)
        {
            _gameTimerLabel = new Label("", "Fonts/DefaultFont") { Color = Color.White, Position = new Point(16, 8) };
            _visionLabel = new Label("Your vision", "Fonts/DefaultFont")
            {
                Color = Color.White,
                Position = new Point(Game.GraphicsDevice.Viewport.Width - 16, 8),
                HorizontalOrigin = MonoGameLib.GUI.Base.HorizontalAlign.Right,
            };

            _map = MapLoader.LoadMap("Content/Maps/MainMap.tmx");
            _map.Layers.First(l => l.Name == "MainLayer").Depth = 0.0f;
            _map.Layers.First(l => l.Name == "Scenery").Depth = 0.5f;
            Entities = _map.Objects.Select(CreateEntity).ToList();
            _newEntities = new List<GamePlayEntity>();
            _oldEntities = new List<GamePlayEntity>();
            Timer = new ContextTimer(TimeSpan.FromMinutes(1));

            SoundEffect music = Game.Content.Load<SoundEffect>("Audio/Music/gameplay.wav");
            _bgmInstance = music.CreateInstance();
            _bgmInstance.IsLooped = true;
        }
        #endregion

        #region Activity Life Cycle
        protected async override Task<int> RunActivity()
        {
            var countdown = CountDown();
            var exitManually = base.RunActivity();
            return await await TaskEx.WhenAny(countdown, exitManually);
        }

        async Task<int> CountDown()
        {
            var finishTimer = UpdateContext.Run(Timer);
            await UpdateContext.Delay(Timer.RemainingTime.Divide(2));
            ShowTrueVision();
            await finishTimer;
            return Score;
        }

        protected override void Activating()
        {
            _bgmInstance.Play();
            base.Activating();
        }

        protected override void Deactivating()
        {
            _bgmInstance.Pause();
            base.Deactivating();
        }
        #endregion

        #region Game Loop
        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit(-1);

            Camera = new CameraInfo(Player.CenterPosition, 2, Game.GraphicsDevice.Viewport);

            foreach (var ent in Entities)
                ent.Update(gameTime);

            foreach (var rem in _oldEntities)
                Entities.Remove(rem);

            Entities.AddRange(_newEntities);
            _oldEntities.Clear();
            _newEntities.Clear();
        }

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch.GraphicsDevice.Clear(MainGame.DefaultBackgroundColor);
            DrawGameScene(gameTime);
            DrawGui(gameTime);
        }

        void DrawGameScene(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch.Begin(Camera, SamplerState.PointClamp);

            _map.Draw(gameTime, SpriteBatch, Vector2.Zero);

            foreach (var ent in Entities)
            {
                ent.LayerDepth = 0.25f + (ent.Position.Y / _map.PixelSize.Y / 200);
                ent.Draw(gameTime, SpriteBatch);
            }

            SpriteBatch.End();
        }

        void DrawGui(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch.Begin();
            _gameTimerLabel.Text = string.Format("Score: {0} Time: {1:00}", Score, Timer.RemainingTime.TotalSeconds);
            _gameTimerLabel.Draw(gameTime, SpriteBatch);
            _visionLabel.Draw(gameTime, SpriteBatch);
            SpriteBatch.End();
        }
        #endregion

        #region Public Methods
        public void AddEntity(GamePlayEntity entity)
        {
            _newEntities.Add(entity);
        }
        public void RemoveEntity(GamePlayEntity entity)
        {
            _oldEntities.Add(entity);
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
        #endregion

        #region Private Methods
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
        void ShowTrueVision()
        {
            foreach (var tileset in _map.Tilesets)
                tileset.Texture = tileset.Texture.AsTrueVision(Game);
            _visionLabel.Text = "Their vision";
            _visionLabel.Color = Color.Red;

            _bgmInstance.Stop();
            SoundEffect music = Game.Content.Load<SoundEffect>("Audio/Music/gameplay-truevision.wav");
            _bgmInstance = music.CreateInstance();
            _bgmInstance.IsLooped = true;
            _bgmInstance.Play();

            IsTrueVision = true;
        }
        #endregion
    }
}
