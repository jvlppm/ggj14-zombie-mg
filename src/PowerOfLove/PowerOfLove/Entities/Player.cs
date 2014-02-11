using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core.Particles;
using MonoGameLib.Core.Sprites;
using PowerOfLove.Activities;
using PowerOfLove.Entities.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerOfLove.Entities
{
    class Player : GamePlayEntity
    {
        static Sprite _loveParticleSprite, biteParticleSprite;

        ParticleEmiter _loveParticleEmiter, _biteParticleEmiter;

        const string SpritePathFormat = "Images/Sprites/player-{0}";
        static Sprite LoadSprite(Game game, string type)
        {
            string spritePath = string.Format(SpritePathFormat, type);
            var sprite = new Sprite(game.Content.Load<Texture2D>(spritePath), new Point(16, 16));
            sprite.AddAnimation("stand", new[] { 6, 7, 8 }, TimeSpan.FromMilliseconds(300), true);
            sprite.AddAnimation("run", new[]{0, 1, 2, 1}, TimeSpan.FromMilliseconds(100), true);
            sprite.AddAnimation("hug", new[] { 9, 10, 11 }, TimeSpan.FromMilliseconds(200), false);
            sprite.CurrentAnimation = "stand";

            return sprite;
        }

        public Player(Game game, GamePlayScreen screen)
            : base(screen)
        {
            Sprite = LoadSprite(game, "normal");
            NormalTexture = Sprite.Texture;
            ZombieTexture = NormalTexture.AsZombie(game);

            Scale = new Vector2(2);
            Behaviors.Add(new TouchControlBehavior(this));
            CollisionBox = new Rectangle(8, 2, 8, 0);


            if (_loveParticleSprite == null)
            {
                var loveTexture = game.Content.Load<Texture2D>("Images/Sprites/heart");
                _loveParticleSprite = new Sprite(loveTexture, 1, 1);
                _loveParticleSprite.AddAnimation("default", new[] { 0 }, TimeSpan.FromSeconds(1), true);
                _loveParticleSprite.Origin = new Vector2(loveTexture.Width / 2, loveTexture.Height / 2);
            }

            _loveParticleEmiter = new ParticleEmiter(game, screen, _loveParticleSprite,
                new[] {
                    new ParticleState { Color = Color.Red, Duration = 500, Scale = 1 },
                    new ParticleState { Color = new Color(Color.White, 0.2f), Scale = 1.5f },
                })
            {
                LayerDepth = 0.0f,
                MillisecondsToEmit = 150,
                Direction= new Vector2(0, -1),
                OpeningAngle = 30,
                ParticleSpeed = 1
            };
        }

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            if (Screen.IsTrueVision)
                Sprite.Texture = ZombieTexture;
            else
                Sprite.Texture = NormalTexture;

            if (IsHugging)
            {
                _loveParticleEmiter.Position = CenterPosition;
                _loveParticleEmiter.Update(gameTime);
                base.Update(gameTime);
                return;
            }

            var en = Screen.NearestToEntity(this, "enemy");
            if (en != null)
            {
                var p = en.Position - Position;
                if (this.GetCollisionRectangle().Intersects(this.GetCollisionRectangle(p)))
                {
                    Hug(en);
                }
            }

            base.Update(gameTime);
        }
        #endregion

        #region Public Methods
        public async void Hug(GamePlayEntity entity)
        {
            var oldPos = entity.Position;
            entity.Position = Position;

            entity.IsHugging = true;
            IsHugging = true;

            entity.Sprite.Effect = Sprite.Effect == SpriteEffects.FlipHorizontally ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var hugs = TaskEx.WhenAll(
                Sprite.PlayAnimation("hug"),
                entity.Sprite.PlayAnimation("hug"));

            await TaskEx.Delay(300);
            entity.TurnIntoFriend();
            Screen.Score++;
            await hugs;

            entity.IsHugging = false;
            IsHugging = false;

            entity.Position = new Vector2(Position.X + ( Sprite.Effect == SpriteEffects.FlipHorizontally? -10 : 10), entity.Position.Y);
        }
        #endregion
    }
}
