using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using MonoGameLib.Core;
using PowerOfLove.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerOfLove
{
    class MainGame : Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }

        public static readonly Color DefaultBackgroundColor = new Color(32, 32, 32);

        public MainGame()
        {
            Window.Title = "Power of Love";
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600
            };

            Content.RootDirectory = "Content";
            GameContent.Initialize(Content);
            SoundManager.SEFolder = "Sounds";
            SoundManager.BGMFolder = "BGM";
        }

        protected override void Initialize()
        {
            base.Initialize();
#if ANDROID
            Window.IsBorderless = true;
            Graphics.IsFullScreen = true;
#else
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
                    case TitleResult.Credits:
                        await ShowCredits(host);
                        break;
                    case TitleResult.HowToPlay:
                        await ShowHowToPlay(host);
                        break;
                    case TitleResult.Play:
                        await RunGamePlay(host);
                        break;
                    case TitleResult.Exit:
                        return;
                }
            }
        }

        Task<TitleResult> ShowTitle(ActivityHost host)
        {
            return host.Run<TitleActivity, TitleResult>();
        }

        Task ShowCredits(ActivityHost host)
        {
            return host.Run<CreditsActivity>();
        }

        Task ShowHowToPlay(ActivityHost host)
        {
            return host.Run<HowToPlayActivity>();
        }

        async Task RunGamePlay(ActivityHost host)
        {
            var result = await host.Run<GamePlayActivity, int>();
            await host.Run<ResultsActivity>(result);
        }
    }
}
