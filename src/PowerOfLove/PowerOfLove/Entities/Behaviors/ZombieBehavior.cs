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

        new GamePlayEntity Entity { get { return (GamePlayEntity)base.Entity; } }
        GamePlayScreen Screen { get{return Entity.Screen; }}

        public ZombieBehavior(Entity entity)
            : base(entity)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Entity.IsHugging)
                return;

            _target = _target ?? Screen.NearestToEntity(Entity, "player");
            if ( _target != null && Entity.Tag == "enemy" )
			{			
				var desiredVelocity = Entity.Position - _target.Position;
				var distance = desiredVelocity.Length();

                desiredVelocity.Normalize();
                Entity.Look(-desiredVelocity);
				
				if( distance < 100 )
				{
                    desiredVelocity = desiredVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 30;
                    Entity.Sprite.CurrentAnimation = "run";
					if (!Screen.TrueVision) {
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

                Entity.Move(desiredVelocity);
			}
			else if ( Entity.Tag == "friend" )
			{
				var desiredVelocitySeek = _target.Position - Entity.Position;
				var distanceSeek = desiredVelocitySeek.Length();

				desiredVelocitySeek.Normalize();
				desiredVelocitySeek *= (float)gameTime.ElapsedGameTime.TotalSeconds * 45;

                Entity.Look(desiredVelocitySeek);

                if (distanceSeek > _minDistance)
                {
                    Entity.Move(desiredVelocitySeek);
                    Entity.Sprite.CurrentAnimation = "run";
                }
                else
                    Entity.Sprite.CurrentAnimation = "stand";
			}
		}
    }
}
