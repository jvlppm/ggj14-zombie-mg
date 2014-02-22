using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGameLib.Core.Sprites;
using MonoGameLib.GUI.Base;
using MonoGameLib.GUI.Components;
using PowerOfLove.Components;
using PowerOfLove.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PowerOfLove.Activities
{
    class LeaderboardsScreen : Activity
    {
        #region Attributes
        GUI _gui;
        Container _playersScorePanel;
        Label _loading;
        Button _modeButton;
        CancellationTokenSource _loadPlayersCancellation;

        readonly float textBasePosY;
        float textPosY;
        Song _music;
        bool _orderByHighscore;
        #endregion

        #region Constructors
        public LeaderboardsScreen(Game game)
            : base(game)
        {
            _gui = new GUI(new Vector2(GraphicsDevice.Viewport.Height / 500f));
            textBasePosY = textPosY = 40 * _gui.Scale.Y;
            CreateHeaders();
            textBasePosY = textPosY;
            ClearPlayersScore();
            CreateLoading();
            CreateButtons(game);

            _loadPlayersCancellation = new CancellationTokenSource();
            LoadRankType(_orderByHighscore);

            _music = Game.Content.Load<Song>("Audio/Music/credits.wav");
        }
        #endregion

        #region GUI
        void CreateHeaders()
        {
            var playerNameLabel = new Label("NAME", "Fonts/DefaultFont")
            {
                Color = Color.Red,
                Position = new Point(Viewport.Width / 8, (int)textPosY)
            };
            _gui.Add(playerNameLabel);

            _gui.Add(new Label("HIGH SCORE", "Fonts/DefaultFont")
            {
                Color = Color.Red,
                HorizontalOrigin = HorizontalAlign.Center,
                Position = new Point((int)(Viewport.Width * 5 / 8.0f - 70 * _gui.Scale.X), (int)textPosY)
            });

            _gui.Add(new Label("TOTAL ZOMBIES", "Fonts/DefaultFont")
            {
                Color = Color.Red,
                HorizontalOrigin = HorizontalAlign.Right,
                Position = new Point((int)(Viewport.Width * 6 / 8.0f + 100 * _gui.Scale.X), (int)textPosY)
            });

            textPosY += playerNameLabel.MeasureSize().Y;
        }

        void CreateLoading()
        {
            _loading = new Label("Loading", "Fonts/DefaultFont")
            {
                Color = Color.Yellow,
                Position = new Point(Viewport.Width / 8, (int)textPosY)
            };
            _gui.Add(_loading);
        }

        void ClearPlayersScore()
        {
            textPosY = textBasePosY;

            if (_playersScorePanel == null)
            {
                _playersScorePanel = new Container();
                _gui.Add(_playersScorePanel);
            }
            else
                _playersScorePanel.Clear();
        }

        void AddPlayerScore(string name, string map, int highScore, int totalZombies)
        {
            _playersScorePanel.AddChildren(new Label(name.Ellipsize(22), "Fonts/DefaultFont")
            {
                Scale = _playersScorePanel.Scale,
                Color = Color.YellowGreen,
                Position = new Point(Viewport.Width / 8, (int)textPosY)
            });

            _playersScorePanel.AddChildren(new Label(highScore.ToString(), "Fonts/DefaultFont")
            {
                Scale = _playersScorePanel.Scale,
                Color = Color.White,
                HorizontalOrigin = HorizontalAlign.Right,
                Position = new Point((int)(Viewport.Width * 5 / 8.0f), (int)textPosY)
            });

            _playersScorePanel.AddChildren(new Label(map.Ellipsize(7), "Fonts/DefaultFont")
            {
                Scale = _playersScorePanel.Scale,
                Color = Color.Yellow,
                HorizontalOrigin = HorizontalAlign.Left,
                Position = new Point((int)(Viewport.Width * 5 / 8.0f - 140 * _gui.Scale.X), (int)textPosY)
            });

            _playersScorePanel.AddChildren(new Label(totalZombies.ToString(), "Fonts/DefaultFont")
            {
                Scale = _playersScorePanel.Scale,
                Color = Color.White,
                HorizontalOrigin = HorizontalAlign.Right,
                Position = new Point((int)(Viewport.Width * 6 / 8.0f + 100 * _gui.Scale.X), (int)textPosY)
            });

            textPosY += 32 * _gui.Scale.Y;
        }

        void CreateButtons(Game game)
        {
            var btnBack = new Button(game, "Return") { HorizontalOrigin = HorizontalAlign.Center };
            btnBack.Position = new Point(
                Viewport.Width * 3 / 4,
                Viewport.Height * 7 / 8);
            btnBack.Clicked += (s, e) => Exit();
            _gui.Add(btnBack);

            _modeButton = new Button(game, "Total") { HorizontalOrigin = HorizontalAlign.Center, IsVisible = false };
            _modeButton.Position = new Point(
                Viewport.Width / 4,
                Viewport.Height * 7 / 8);
            _modeButton.Clicked += (s, e) =>
            {
                LoadRankType(_orderByHighscore);
            };
            _gui.Add(_modeButton);
        }

        async void LoadRankType(bool orderByHighscore)
        {
            var oldButtonMode = orderByHighscore;
            var scoreType = oldButtonMode ? PowerOfLoveService.RankType.HighScore
                                            : PowerOfLoveService.RankType.TotalZombies;

            _loadPlayersCancellation.Cancel();
            _loadPlayersCancellation = new CancellationTokenSource();
            var cancel = _loadPlayersCancellation.Token;
            try
            {
                _modeButton.IsVisible = false;
                await Task.WhenAll(LoadPlayerScores(scoreType, cancel), Task.Delay(1000));
                if (cancel.IsCancellationRequested)
                    return;
                SetModeButtonAction(!oldButtonMode);
            }
            catch (Exception)
            {
            }
            _modeButton.IsVisible = true;
        }

        private void SetModeButtonAction(bool orderByHighscore)
        {
            _orderByHighscore = orderByHighscore;
            _modeButton.Text = _orderByHighscore ? "Best" : "Total";
        }
        #endregion

        #region Activity Life-Cycle
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
        #endregion

        #region Game Loop
        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch.GraphicsDevice.Clear(MainGame.DefaultBackgroundColor);
            _gui.Draw(gameTime, SpriteBatch);
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
            _gui.Update(gameTime);
        }
        #endregion

        public async Task LoadPlayerScores(PowerOfLoveService.RankType scoreType, CancellationToken cancellation)
        {
            var facebookId = Facebook.Instance.UserId;
            var access_token = Facebook.Instance.AccessToken;

            if (facebookId == null || access_token == null)
            {
                _loading.Text = "Not logged in";
                return;
            }

            _loading.IsVisible = true;
            _loading.Text = "Loading";

            ClearPlayersScore();

            var availableScreenSize = (Viewport.Height * 7 / 8 - 32 * _gui.Scale.Y * 2)  - textPosY;
            var count = availableScreenSize / (32 * _gui.Scale.Y);

#if ANDROID
            try
            {
                var getPlayers = PowerOfLoveService.Instance.LoadRankingsAsync(facebookId, scoreType, (int)count, access_token);
                var players = await getPlayers.On(UpdateContext);

                if (cancellation.IsCancellationRequested)
                    return;

                _loading.IsVisible = false;

                foreach (var playerInfo in players)
                    AddPlayerScore(playerInfo.Name, playerInfo.MapName, playerInfo.HighScore, playerInfo.TotalZombies);

            }
            catch(Exception)
            {
                _loading.IsVisible = true;
                _loading.Text = "Error";
                throw;
            }
#elif DEBUG
            for(int i = 0; i < count; i++)
                AddPlayerScore("Test name", "Android", 18, 200);
#endif
        }
    }
}
