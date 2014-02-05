using MonoGameLib.Core.Entities;
using PowerOfLove.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Entities.Behaviors
{
    class GamePlayEntity : Entity
    {
        public GamePlayScreen Screen { get; private set; }

        public GamePlayEntity(GamePlayScreen screen)
        {
            if (screen == null)
                throw new ArgumentNullException("screen");
            Screen = screen;
        }
    }
}
