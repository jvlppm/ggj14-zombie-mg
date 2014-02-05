using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using PowerOfLove.Entities.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Activities
{
    class GamePlayScreen : Activity<int>
    {
        public IEnumerable<GamePlayEntity> Entities { get; private set; }
        public bool IsEvil { get; private set; }

        public GamePlayScreen(Game game)
            : base(game)
        {
            Entities = new List<GamePlayEntity>();
        }

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public GamePlayEntity NearestToEntity(GamePlayEntity baseEntity, string tag)
        {
            return Entities.Where(s => s != baseEntity && s.Tag == tag)
                           .OrderBy(s => (s.Position - baseEntity.Position).LengthSquared())
                           .FirstOrDefault();
        }
    }
}
