using MonoGameLib.Core.Entities;
using PowerOfLove.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Entities.Behaviors
{
    abstract class GameEntityBehavior : Behavior
    {
        protected new GamePlayEntity Entity { get { return (GamePlayEntity)base.Entity; } }
        protected GamePlayScreen Screen { get { return Entity.Screen; } }

        protected GameEntityBehavior(GamePlayEntity entity)
            : base(entity)
        {

        }
    }
}
