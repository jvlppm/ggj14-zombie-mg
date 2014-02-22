using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGameLib.GUI.Base;
using MonoGameLib.GUI.Components;
using PowerOfLove.Components;

namespace PowerOfLove.Activities
{
    class HowToPlayScreen : Activity
    {
        #region Attributes
        GUI _gui;
        Song _music;
        #endregion

        #region Constructors
        public HowToPlayScreen(Game game)
            : base(game)
        {
            _gui = new GUI(new Vector2(GraphicsDevice.Viewport.Height / 500f))
            {
                CreateObjective(game),
                CreateKeyBindings(game),
                CreateBackButton(game)
            };
            _music = Game.Content.Load<Song>("Audio/Music/help.wav");
        }
        #endregion

        #region GUI
        Component CreateObjective(Game game)
        {
            return new Label("Objective: You have to save as many people as you can.\r\n           Hug them to save'em with your love.", "Fonts/DefaultFont")
            {
                Color = Color.White,
                HorizontalOrigin = HorizontalAlign.Center,
                VerticalOrigin = VerticalAlign.Middle,
                Position = new Point(game.GraphicsDevice.Viewport.Width / 2, (int)(game.GraphicsDevice.Viewport.Height / 4 * 1.7f - 60))
            };
        }
        Component CreateKeyBindings(Game game)
        {
            return new Label("Movement: Touch / Mouse click\r\n", "Fonts/DefaultFont")
            {
                Color = Color.White,
                HorizontalOrigin = HorizontalAlign.Center,
                VerticalOrigin = VerticalAlign.Middle,
                Position = new Point(game.GraphicsDevice.Viewport.Width / 2, (int)(game.GraphicsDevice.Viewport.Height / 4 * 2.7f - 60))
            };
        }
        Component CreateBackButton(Microsoft.Xna.Framework.Game game)
        {
            var btnBack = new Button(game, "Return") { HorizontalOrigin = HorizontalAlign.Center };
            btnBack.Position = new Point(
                Game.GraphicsDevice.Viewport.Width / 2,
                Game.GraphicsDevice.Viewport.Height * 7 / 8);
            btnBack.Clicked += (s, e) => Exit();
            return btnBack;
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
        protected override void Draw(GameTime gameTime)
        {
            SpriteBatch.GraphicsDevice.Clear(MainGame.DefaultBackgroundColor);
            _gui.Draw(gameTime, SpriteBatch);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            _gui.Update(gameTime);
        }
        #endregion
    }
}
