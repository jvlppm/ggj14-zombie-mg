using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using MonoGameLib.Core;
using PowerOfLove.Activities;
using System.Threading.Tasks;

namespace PowerOfLove
{
    public class MainGame : Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }

        public static readonly Color DefaultBackgroundColor = new Color(32, 32, 32);

        public MainGame()
        {
            //Window.Title = "Power of Love";
            Graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true,
                //PreferredBackBufferWidth = 800,
                //PreferredBackBufferHeight = 600,
                SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight
            };

#if ANDROID
            Content.RootDirectory = "Content";
#else
            Content.RootDirectory = "Assets/Content";
#endif
            GameContent.Initialize(Content);
            SoundManager.SEFolder = "Audio/SoundEffects";
            SoundManager.BGMFolder = "Audio/Music";
            MediaPlayer.IsRepeating = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
#if !ANDROID
            IsMouseVisible = true;
#endif
            this.Play(GameMain);
        }

        async Task GameMain(ActivityHost host)
        {
            while (true)
            {
                switch (await ShowTitle(host))
                {
                    case TitleScreen.Result.Credits:
                        await ShowCredits(host);
                        break;
                    case TitleScreen.Result.HowToPlay:
                        await ShowHowToPlay(host);
                        break;
                    case TitleScreen.Result.Play:
                        await RunGamePlay(host);
                        break;
                    case TitleScreen.Result.Leaderboards:
                        await ShowLeaderboards(host);
                        break;
                    case TitleScreen.Result.Exit:
                        return;
                }
            }
        }

        ContextTaskAwaitable<TitleScreen.Result> ShowTitle(ActivityHost host)
        {
            return host.Run<TitleScreen, TitleScreen.Result>();
        }

        ContextTaskAwaitable ShowCredits(ActivityHost host)
        {
            return host.Run<CreditsScreen>();
        }

        ContextTaskAwaitable ShowHowToPlay(ActivityHost host)
        {
            return host.Run<HowToPlayScreen>();
        }

        ContextTaskAwaitable ShowLeaderboards(ActivityHost host)
        {
            return host.Run<LeaderboardsScreen>();
        }

        async Task RunGamePlay(ActivityHost host)
        {
            var result = await host.Run<GamePlayScreen, int>();
            if(result >= 0)
                await host.Run<ResultsScreen>(result);
        }
    }
}
