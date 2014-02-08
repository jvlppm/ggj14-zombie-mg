﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        const string spritePathFormat = "Images/Sprites/player-{0}";
        static Sprite LoadSprite(Game game, string type)
        {
            string spritePath = string.Format(spritePathFormat, type);
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
            NormalSprite = LoadSprite(game, "normal");
            EvilSprite = LoadSprite(game, "zombie");

            Scale = new Vector2(2);
            Behaviors.Add(new ControllableBehavior(this));
        }

        public void RandomZombieNpcMessage()
        {
            throw new NotImplementedException();
        }

        public void RandomHumanNpcMessage()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if (Screen.IsEvil)
                Sprite = EvilSprite;
            else
                Sprite = NormalSprite;

            if (IsHugging)
            {
                base.Update(gameTime);
                return;
            }

            var en = Screen.NearestToEntity(this, "enemy");
            if (en != null)
            {
                var p = en.Position - Position;
                if (p.LengthSquared() < 160)
                {
                    Hug(en);
                }
            }

            base.Update(gameTime);
        }

        #region Public Methods
        public async void Hug(GamePlayEntity entity)
        {
            var oldPos = entity.Position;
            entity.Position = Position;
            entity.IsHugging = true;
            IsHugging = true;
            entity.Sprite.Effect = Sprite.Effect == SpriteEffects.FlipHorizontally ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            await TaskEx.WhenAll(
                Sprite.PlayAnimation("hug"),
                entity.Sprite.PlayAnimation("hug"));
            entity.TurnIntoFriend();
            IsHugging = false;
            entity.Position = new Vector2(oldPos.X, entity.Position.Y);
            entity.IsHugging = false;
        }
        #endregion
    }
}