using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core;
using MonoGameLib.Core.Sprites;
using MonoGameLib.GUI.Base;
using MonoGameLib.Tiled;
using PowerOfLove.Activities;
using PowerOfLove.Entities.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerOfLove.Entities
{
    class NPC : GamePlayEntity
    {
        const string SpritePathFormat = "Images/Sprites/npc-{0}-{1}";
        static Sprite LoadSprite(Game game, string type, int id)
        {
            string spritePath = string.Format(SpritePathFormat, type, id.ToString("D2"));
            var sprite = new Sprite(game.Content.Load<Texture2D>(spritePath), new Point(16, 16));
            sprite.AddAnimation("stand", new[] { 6, 7, 8 }, TimeSpan.FromMilliseconds(300), true);
            sprite.AddAnimation("run", new[] { 0, 1, 2, 1 }, TimeSpan.FromMilliseconds(100), true);
            sprite.AddAnimation("hug", new[] { 9, 10, 11 }, TimeSpan.FromMilliseconds(200), false);
            sprite.CurrentAnimation = "stand";

            return sprite;
        }

        string _currentHumanMessage, _currentZombieMessage;
        SpriteFont _screamFont;
        SoundEffect _zombieScreamSound, _humanScreamSound;

        public string Name { get; set; }

        public NPC(Game game, GamePlayScreen screen, int npcSpriteId)
            : base(screen)
        {
            _screamFont = game.Content.Load<SpriteFont>("Fonts/DefaultFont");

            Sprite = LoadSprite(game, "normal", npcSpriteId);
            NormalTexture = Sprite.Texture;
            ZombieTexture = NormalTexture.AsZombie(game);

            Scale = new Vector2(2);
            Behaviors.Add(new ZombieBehavior(this));
            CollisionBox = new Rectangle(8, 2, 8, 0);

            _zombieScreamSound = game.Content.Load<SoundEffect>("Audio/Effects/zombie01.wav");
            _humanScreamSound = game.Content.Load<SoundEffect>(string.Format("Audio/Effects/no{0}.wav", npcSpriteId % 2));
        }

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            if (IsZombie)
                Sprite.Texture = ZombieTexture;
            else
                Sprite.Texture = NormalTexture;

            base.Update(gameTime);
        }

        public override void DrawOverMap(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawCurrentMessage(spriteBatch);
            if (!IsFriend)
                DrawName(spriteBatch);
        }

        private void DrawCurrentMessage(SpriteBatch spriteBatch)
        {
            if (_currentHumanMessage != null && _currentZombieMessage != null)
            {
                var message = IsZombie ? _currentZombieMessage : _currentHumanMessage;
                var color = !IsZombie ? Color.Yellow : Color.FromNonPremultiplied(0xff, 0x55, 0x55, 0xff);
                var textSize = _screamFont.MeasureString(message);
                var origin = AlignExtensions.ToVector(HorizontalAlign.Center, VerticalAlign.Bottom);
                spriteBatch.DrawString(_screamFont, message, new Vector2(CenterPosition.X, Position.Y), color, 0, textSize * origin, 0.5f, SpriteEffects.None, 1f);
            }
        }

        private void DrawName(SpriteBatch spriteBatch)
        {
            if (Name != null)
            {
                var message = Name;
                var color = Color.White;
                var textSize = _screamFont.MeasureString(message);
                var origin = AlignExtensions.ToVector(HorizontalAlign.Center, VerticalAlign.Top);
                spriteBatch.DrawString(_screamFont, message, new Vector2(CenterPosition.X, Position.Y + Size.Y * Scale.Y), color, 0, textSize * origin, 0.4f, SpriteEffects.None, 1f);
            }
        }
        #endregion

        #region Public Methods
        public override void BeginTransformation()
        {
            if (IsZombie)
                _zombieScreamSound.Play();
            else
                _humanScreamSound.Play();
        }

        public override void TurnIntoFriend()
        {
            base.TurnIntoFriend();
            _currentZombieMessage = null;
            _currentHumanMessage = null;
            Greet();
        }

        public async void Scream()
        {
            if (_currentZombieMessage != null && _currentHumanMessage != null)
                return;

            _currentZombieMessage = new[] { "GAHH!", "BLHRRHHRH!", "BRAINS", "BR41NNSS", "#GGJCWB" }.Random();
            _currentHumanMessage = new[] { "HEEEELLLLPPP A ZOMBIEEEE", "OH NO A PROGRAMER BROKE FREE!", "APOCALYPSE IS COMING!", "Don't get any closer!", "NOOOOOOOOOOOO!!!!!!" }.Random();

            var message = _currentZombieMessage;
            await TaskEx.Delay(TimeSpan.FromSeconds(2));
            if (message == _currentZombieMessage)
                _currentZombieMessage = _currentHumanMessage = null;
        }

        public async void Greet()
        {
            if (_currentZombieMessage != null && _currentHumanMessage != null)
                return;

            _currentZombieMessage = new[] { "AGHHH", "HE GOT ME!", "BURRRRR", "BRAAAAINNNNNNSSS", "..............." }.Random();
            _currentHumanMessage = new[] { "Thank you!", "I love you Mr!", "I almost died due to fatigue! Can't program so much!", "Help the others please!", "I'll follow you!" }.Random();

            var message = _currentZombieMessage;
            await TaskEx.Delay(TimeSpan.FromSeconds(2));
            if (message == _currentZombieMessage)
                _currentZombieMessage = _currentHumanMessage = null;
        }
        #endregion
    }
}
