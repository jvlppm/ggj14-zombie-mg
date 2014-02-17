using MonoGameLib.Core.Entities;
using MonoGameLib.GUI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfLove.Components
{
    class EntityComponent : Component
    {
        public Entity Entity { get; private set; }

        public EntityComponent(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            Entity = entity;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Entity.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            Entity.Draw(gameTime, spriteBatch);
        }
    }
}
