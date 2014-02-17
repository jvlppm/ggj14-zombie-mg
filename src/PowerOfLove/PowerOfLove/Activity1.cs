using Android.App;
using Android.Content.PM;
using Android.OS;
using PowerOfLove;

namespace GameTest.Android
{
    [Activity(Label = "GameTest.Android"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = global::Android.Content.PM.LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.SensorLandscape
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
    public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
    {
        public static global::Android.Content.Res.AssetManager AssetsManager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            AssetsManager = Assets;
            MainGame.Activity = this;
            var g = new MainGame();
            SetContentView(g.Window);
            g.Run();
        }
    }
}

