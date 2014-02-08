using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core.Sprites;
using MonoGameLib.Tiled;
using PowerOfLove.Activities;
using PowerOfLove.Entities.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Entities
{
    class NPC : GamePlayEntity
    {
        const string spritePathFormat = "Images/Sprites/npc-{0}-{1}";
        static Sprite LoadSprite(Game game, string type, int id)
        {
            string spritePath = string.Format(spritePathFormat, type, id.ToString("D2"));
            var sprite = new Sprite(game.Content.Load<Texture2D>(spritePath), new Point(16, 16));
            sprite.AddAnimation("stand", new[] { 6, 7, 8 }, TimeSpan.FromMilliseconds(300), true);
            sprite.AddAnimation("run", new[] { 0, 1, 2, 1 }, TimeSpan.FromMilliseconds(100), true);
            sprite.AddAnimation("hug", new[] { 9, 10, 11 }, TimeSpan.FromMilliseconds(200), false);
            sprite.CurrentAnimation = "stand";

            return sprite;
        }


        public NPC(Game game, GamePlayScreen screen, int npcSpriteId)
            : base(screen)
        {
            Sprite = LoadSprite(game, "normal", npcSpriteId);
            EvilSprite = LoadSprite(game, "zombie", npcSpriteId);
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
