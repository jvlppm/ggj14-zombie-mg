using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core.Extensions;
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


            _loveParticleEmiter = new ParticleEmiter(game, screen, "Images/Sprites/heart",
                new[] {
                    new ParticleState { Color = Color.Red, Duration = 500, Scale = 1 },
                    new ParticleState { Color = new Color(Color.White, 0f), Scale = 2f },
                })
            {
                MillisecondsToEmit = 150,
                Direction= new Vector2(0, -1),
                OpeningAngle = 90,
                ParticleSpeed = 40
            };

            _biteParticleEmiter = new ParticleEmiter(game, screen, "Images/Sprites/blood",
                new[] {
                    new ParticleState { Color = Color.Red, Duration = 700, Scale = 1 },
                    new ParticleState { Color = Color.Transparent, Scale = 0.5f },
                })
            {
                MillisecondsToEmit = 30,
                OpeningAngle = 35,
                ParticleSpeed = 50,
                ParticleGravity = new Vector2(0, 98f)
            };
        }

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            if (IsZombie)
                Sprite.Texture = ZombieTexture;
            else
                Sprite.Texture = NormalTexture;

            if (IsHugging)
            {
                if (Screen.IsTrueVision)
                    EmitBlood(gameTime);
                else
                    EmitHearts(gameTime);

                base.Update(gameTime);
                return;
            }

            HugOnTouch("enemy");

            base.Update(gameTime);
        }

        private void HugOnTouch(string category)
        {
            var en = Screen.NearestToEntity(this, category);
            if (en != null)
            {
                var p = en.Position - Position;
                if (this.GetCollisionRectangle().Intersects(this.GetCollisionRectangle(p)))
                    Hug(en);
            }
        }
        #endregion

        #region Private Methods
        async void Hug(GamePlayEntity entity)
        {
            var oldPos = entity.Position;
            entity.Position = Position;

            entity.IsHugging = true;
            IsHugging = true;
            entity.BeginTransformation();

            entity.Sprite.Effect = Sprite.Effect == SpriteEffects.FlipHorizontally ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var hugs = TaskEx.WhenAll(
                Sprite.PlayAnimation("hug"),
                entity.Sprite.PlayAnimation("hug"));

            await TaskEx.Delay(TimeSpan.FromSeconds(0.3));
            entity.TurnIntoFriend();
            Screen.Score++;
            await hugs;

            entity.IsHugging = false;
            IsHugging = false;

            entity.Position = new Vector2(Position.X + (Sprite.Effect == SpriteEffects.FlipHorizontally ? -10 : 10), entity.Position.Y);
        }

        void EmitBlood(GameTime gameTime)
        {
            int bloodDirection = 1;
            if (Sprite.Effect == SpriteEffects.FlipHorizontally)
                bloodDirection = -1;
            _biteParticleEmiter.Direction = Vector2Extension.AngleToVector2(MathHelper.ToRadians(45 * bloodDirection));
            _biteParticleEmiter.Position = CenterPosition + new Vector2(bloodDirection * 9, -4);
            _biteParticleEmiter.Update(gameTime);
        }

        void EmitHearts(GameTime gameTime)
        {
            _loveParticleEmiter.Position = CenterPosition + new Vector2(0, -8);
            _loveParticleEmiter.Update(gameTime);
        }
        #endregion
    }
}
