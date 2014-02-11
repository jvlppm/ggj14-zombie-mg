using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerOfLove.Entities;
using PowerOfLove.Activities;
using Jv.Games.Xna.Async;
using Jv.Games.Xna.Async.Core;

namespace MonoGameLib.Core.Particles
{
    class Particle : GamePlayEntity
    {
        #region Events
        public event EventHandler OnDecay;
        #endregion

        #region Properties
        public float Opacity { get; set; }

        public Vector2 Direction { get; set; }
        public float Speed { get; set; }
        #endregion Properties

        #region Constructor
        public Particle(GamePlayScreen screen, Texture2D texture, Vector2 position, List<ParticleState> states)
            : base(screen)
        {
            Position = position;
            Opacity = 1;
            LayerDepth = 1;
            screen.DrawContext.Send(c => Animate(c, states));

            Sprite = new Sprites.Sprite(texture, 1, 1);
            Sprite.AddAnimation("default", new[] { 0 }, TimeSpan.Zero, true);
        }
        #endregion Constructor

        #region Particle Methods
        public virtual void CalculatePosition()
        {
            Position += (Direction * Speed);
        }
        #endregion Particle Methos

        private async void Animate(AsyncContext context, List<ParticleState> states)
        {
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                var nextState = states.Count > i + 1 ? states[i + 1] : null;

                Color = state.Color * Opacity;
                Scale = new Vector2(state.Scale);

                if (state.Duration <= 0)
                    continue;

                if (nextState == null)
                    await context.Delay(TimeSpan.FromMilliseconds(state.Duration));
                else
                {
                    await TaskEx.WhenAll(
                        context.Animate(TimeSpan.FromMilliseconds(state.Duration), state.Scale, nextState.Scale, v => Scale = new Vector2(v)),
                        context.Animate(TimeSpan.FromMilliseconds(state.Duration), state.Color, nextState.Color, v => Color = v));
                }
            }
            Screen.RemoveEntity(this);
            if (OnDecay != null)
                OnDecay(this, EventArgs.Empty);
        }
    }
}
