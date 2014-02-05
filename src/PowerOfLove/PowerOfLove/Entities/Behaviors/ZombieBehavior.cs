using Microsoft.Xna.Framework;
using MonoGameLib.Core;
using MonoGameLib.Core.Entities;
using PowerOfLove.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Entities.Behaviors
{
    class ZombieBehavior : Behavior
    {
        Entity _target;
        private float _minDistance = RandomNumberGenerator.Next() * 90 + 10;
        GamePlayScreen Screen { get{return ((GamePlayEntity)Entity).Screen; }}

        public ZombieBehavior(Entity entity)
            : base(entity)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _target = _target ?? Screen.NearestToEntity((GamePlayEntity)Entity, "player");
            if ( _target != null && Entity.Tag == "enemy" )
			{			
				var desiredVelocity = Entity.Position - _target.Position;
				var distance = desiredVelocity.Length();
				
				if( distance < 100 )
				{
					desiredVelocity.Normalize();
                    desiredVelocity = desiredVelocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 30;
                    Entity.Sprite.CurrentAnimation = "run";
					if (!Screen.IsEvil) {
						((NPC)Entity).RandomZombieNpcMessage();
					}
					else {
						((NPC)Entity).RandomHumanNpcMessage();
					}
				}
				else
				{
					desiredVelocity = Vector2.Zero;
					Entity.Sprite.CurrentAnimation = "stand";
				}
				
				Entity.Position = Entity.Position + desiredVelocity;
			}
			else if ( Entity.Tag == "npc" )
			{
				var desiredVelocitySeek = _target.Position - Entity.Position;
				var distanceSeek = desiredVelocitySeek.Length();

				desiredVelocitySeek.Normalize();
				desiredVelocitySeek = desiredVelocitySeek * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 45;
				Entity.Sprite.CurrentAnimation = "run";
				
                if(distanceSeek > _minDistance)
                    Entity.Position = Entity.Position + desiredVelocitySeek;
			}
		}
    }
}
