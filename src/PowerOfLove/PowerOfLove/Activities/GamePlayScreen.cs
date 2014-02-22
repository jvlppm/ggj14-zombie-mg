using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGameLib.Core;
using MonoGameLib.Core.Extensions;
using MonoGameLib.GUI.Components;
using MonoGameLib.Tiled;
using PowerOfLove.Components;
using PowerOfLove.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if ANDROID
using PowerOfLove.Helpers;
#endif

namespace PowerOfLove.Activities
{
    class GamePlayScreen : Activity<int>
    {
        #region Attributes
        Map _map;
        Label _gameTimerLabel, _visionLabel;
        GUI _gui;
        List<GamePlayEntity> _newEntities, _oldEntities;
        Song _music;
        #endregion

        #region Properties
        public ContextTimer Timer { get; private set; }
        public ContextTimer TrueVisionTimer { get; private set; }
        public int Score { get; set; }

        public List<GamePlayEntity> Entities { get; private set; }
        public GamePlayEntity Player { get; private set; }
        public bool IsTrueVision { get; private set; }
        public CameraInfo Camera;
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
            _gui = new GUI(new Vector2(GraphicsDevice.Viewport.Height / 500f))
            {
                _gameTimerLabel,
                _visionLabel
            };

#if ANDROID
            _map = MapLoader.LoadMap(game, "Content/Maps/MainMap.tmx");
#else
            _map = MapLoader.LoadMap(game, "Assets/Content/Maps/MainMap.tmx");
#endif
            _map.Layers.First(l => l.Name == "MainLayer").Depth = 0.0f;
            _map.Layers.First(l => l.Name == "Scenery").Depth = 0.5f;
            Entities = _map.Objects.Select(CreateEntity).ToList();
            _newEntities = new List<GamePlayEntity>();
            _oldEntities = new List<GamePlayEntity>();

            Timer = new ContextTimer(TimeSpan.FromMinutes(1));
            TrueVisionTimer = new ContextTimer(Timer.Duration.Divide(2));

            _music = Game.Content.Load<Song>("Audio/Music/gameplay.wav");

            Camera = new CameraInfo(Vector2.Zero, Game.GraphicsDevice.Viewport.Height / 480.0f * 2, Game.GraphicsDevice.Viewport);
        }
        #endregion

        #region Activity Life Cycle
        protected async override Task<int> RunActivity()
        {
#if ANDROID
            LoadFriendNamesAsync();
#endif
            var countdown = CountDown();
            var exitManually = base.RunActivity();
            var firstToComplete = await TaskEx.WhenAny(countdown, exitManually);
            return await firstToComplete.On(TaskEx.CurrentContext);
        }

        async Task<int> CountDown()
        {
            var finishTimer = UpdateContext.Run(Timer);
            await UpdateContext.Run(TrueVisionTimer);
            ShowTrueVision();
            await finishTimer;
            return Score;
        }

        protected override void Activating()
        {
            MediaPlayer.Play(_music);
            base.Activating();
        }

        protected override void Deactivating()
        {
            MediaPlayer.Stop();
            base.Deactivating();
        }

#if ANDROID
        async void LoadFriendNamesAsync()
        {
            try
            {
                var friendNames = await Facebook.Instance.LoadFriendNamesAsync();
                foreach(var ent in Entities.OfType<NPC>())
                    ent.Name = friendNames.Random();
            }
            catch (Exception) { }
        }
#endif
        #endregion

        #region Game Loop
        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit(-1);

            Camera.Position = Player.CenterPosition;
            var cameraRegion = Camera.GetArea();
            var screenEntities = Entities.Where(e => cameraRegion.Intersects(e.GetCollisionRectangle()));

            foreach (var ent in screenEntities)
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

            _map.Layers[0].Draw(SpriteBatch, Camera);

            var cameraRegion = Camera.GetArea();

            var screenEntities = Entities.Where(e => cameraRegion.Intersects(e.GetCollisionRectangle()));

            foreach (var ent in screenEntities.OrderBy(e => e.Position.Y))
                ent.Draw(gameTime, SpriteBatch);

            _map.Layers[1].Draw(SpriteBatch, Camera);

            foreach (var ent in screenEntities.OrderBy(e => e.Position.Y))
                ent.DrawOverMap(gameTime, SpriteBatch);

            SpriteBatch.End();
        }

        void DrawGui(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _gameTimerLabel.Text = string.Format("Score: {0} Time: {1:00}", Score, Timer.RemainingTime.TotalSeconds);
            _gui.Draw(gameTime, SpriteBatch);
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

        public void IncreaseTimer(TimeSpan ammount)
        {
            Timer.Duration += ammount;
            TrueVisionTimer.Duration += ammount;
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

            _music = Game.Content.Load<Song>("Audio/Music/gameplay-truevision.wav");
            MediaPlayer.Play(_music);

            IsTrueVision = true;
        }
        #endregion
    }
}
