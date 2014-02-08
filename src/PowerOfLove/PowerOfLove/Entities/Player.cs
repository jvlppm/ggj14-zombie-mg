﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core.Sprites;
using PowerOfLove.Activities;
using PowerOfLove.Entities.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            Sprite = LoadSprite(game, "normal");
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
    }
}
