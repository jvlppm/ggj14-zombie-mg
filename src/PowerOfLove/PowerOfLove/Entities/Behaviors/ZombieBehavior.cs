using Microsoft.Xna.Framework;
using MonoGameLib.Core;
using MonoGameLib.Core.Entities;
using PowerOfLove.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerOfLove.Entities;

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
            if (((GamePlayEntity)Entity).IsHugging)
                return;

            _target = _target ?? Screen.NearestToEntity((GamePlayEntity)Entity, "player");
            if ( _target != null && Entity.Tag == "enemy" )
			{			
				var desiredVelocity = Entity.Position - _target.Position;
				var distance = desiredVelocity.Length();

                desiredVelocity.Normalize();
                ((GamePlayEntity)Entity).Look(-desiredVelocity);
				
				if( distance < 100 )
				{
                    desiredVelocity = desiredVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 30;
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

                ((GamePlayEntity)Entity).Move(desiredVelocity);
			}
			else if ( Entity.Tag == "npc" )
			{
				var desiredVelocitySeek = _target.Position - Entity.Position;
				var distanceSeek = desiredVelocitySeek.Length();

				desiredVelocitySeek.Normalize();
				desiredVelocitySeek *= (float)gameTime.ElapsedGameTime.TotalSeconds * 45;
				Entity.Sprite.CurrentAnimation = "run";

                ((GamePlayEntity)Entity).Look(desiredVelocitySeek);
				
                if(distanceSeek > _minDistance)
                    ((GamePlayEntity)Entity).Move(desiredVelocitySeek);
			}
		}
    }
}
