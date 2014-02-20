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
using System;

namespace PowerOfLove.Activities
{
    class CreditsScreen : Activity
    {
        #region Attributes
        GUI _gui;
        float textBasePosY;
        float textPosY;
        Song _music;
        #endregion

        #region Constructors
        public CreditsScreen(Game game)
            : base(game)
        {
            _gui = new GUI(new Vector2(GraphicsDevice.Viewport.Height / 500f));
            textBasePosY = 40 * _gui.Scale.Y;
            textPosY = textBasePosY;
            CreateCategory("Game Design", "Diogo Muller de Miranda\r\nRicardo Takeda");
            CreateCategory("Developers ", "Diogo Muller de Miranda\r\nJoao Vitor Pietsiaki Moraes\r\nEric Onuki");
            CreateCategory("Game Art", "Diogo Muller de Miranda\r\nRicardo Takeda");
            CreateCategory("Music and Sound Effects", "Ricardo Takeda");

            CreateBackButton(game);
            CreateZombies();

            _music = Game.Content.Load<Song>("Audio/Music/credits.wav");
        }
        #endregion

        #region GUI
        void CreateCategory(string title, string text)
        {

            var textPosX = Game.GraphicsDevice.Viewport.Width / 2 - 80 * _gui.Scale.X;
            var cat1Title = new Label(title, "Fonts/DefaultFont")
            {
                //FontSize = 24,
                Color = Color.YellowGreen,
                Position = new Point((int)textPosX, (int)textPosY)
            };

            _gui.Add(cat1Title);

            textPosY += cat1Title.MeasureSize().Y;

            if (textPosX == 0)
                textPosX = cat1Title.Position.X;

            var cat1Text = new Label(text, "Fonts/DefaultFont")
            {
                Color = Color.White,
                Position = new Point((int)textPosX, (int)textPosY)
            };

            _gui.Add(cat1Text);
            textPosY += cat1Text.MeasureSize().Y;
        }

        void CreateZombies()
        {
            var cZombie = new[] { "npc-zombie-01", "npc-zombie-02", "npc-zombie-03", "npc-zombie-04", "player-zombie" };
            var cNormal = new[] { "npc-normal-01", "npc-normal-02", "npc-normal-03", "npc-normal-04", "player-normal" };

            var classes = new[] { cZombie, cNormal };

            var zombieX = Viewport.Width / 4 - 80 * _gui.Scale.X;

            foreach (var c in classes)
            {
                float zombieY = (int)textBasePosY;
                float spaceY = (textPosY - textBasePosY) / cZombie.Length - 64 * _gui.Scale.Y;

                foreach (var sprite in c)
                {
                    var texture = Game.Content.Load<Texture2D>("Images/Sprites/" + sprite);
                    var s = new Sprite(texture, new Point(16, 16));
                    s.AddAnimation("dance", new int[] { 0, 1, 2, 3, 4, 5 }, TimeSpan.FromMilliseconds(200));
                    var comp = new SpriteComponent(s)
                    {
                        Position = new Point((int)zombieX, (int)zombieY),
                        Scale = new Vector2(4)
                    };
                    _gui.Add(comp);
                    zombieY += 64 * _gui.Scale.Y + spaceY;
                }
                zombieX += 80;
            }
        }

        void CreateBackButton(Game game)
        {
            var btnBack = new Button(game, "Return") { HorizontalOrigin = HorizontalAlign.Center };
            btnBack.Position = new Point(
                Viewport.Width / 2,
                Viewport.Height * 7 / 8);
            btnBack.Clicked += (s, e) => Exit();
            _gui.Add(btnBack);
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
    }
}
