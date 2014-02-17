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
using MonoGameLib.Core.Sprites;

namespace MonoGameLib.Core.Particles
{
    class Particle : GamePlayEntity
    {
        #region Events
        public event EventHandler OnDecay;
        #endregion

        #region Properties
        public float Opacity { get; set; }
        public Vector2 Gravity { get; set; }
        #endregion Properties

        #region Constructor
        public Particle(GamePlayScreen screen, Sprite sprite, Vector2 position, IEnumerable<ParticleState> states)
            : base(screen)
        {
            Position = position;
            Opacity = 1;
            LayerDepth = 1;
            screen.DrawContext.Send(c => Animate(c, states));
            Color = Color.White;

            Sprite = sprite;
            Friction = Vector2.Zero;
        }
        #endregion Constructor

        #region Particle Methods
        public override void Update(GameTime gameTime)
        {
            ApplyAcceleration(Gravity);
            base.Update(gameTime);
        }
        #endregion Particle Methos

        private async void Animate(AsyncContext context, IEnumerable<ParticleState> states)
        {
            var en = states.GetEnumerator();

            while (en.MoveNext())
            {
                var state = en.Current;
                Color = state.Color * Opacity;
                Scale = new Vector2(state.Scale);

                if (state.Duration <= 0)
                    continue;

                if (!en.MoveNext())
                    await context.Delay(TimeSpan.FromMilliseconds(state.Duration));
                else
                {
                    var nextState = en.Current;
                    await TaskEx.WhenAll<TimeSpan>(
                        (Task<TimeSpan>)context.Animate(TimeSpan.FromMilliseconds(state.Duration), state.Scale, nextState.Scale, v => Scale = new Vector2(v)),
                        (Task<TimeSpan>)context.Animate(TimeSpan.FromMilliseconds(state.Duration), state.Color, nextState.Color, v => Color = v));
                }
            }
            Screen.RemoveEntity(this);
            if (OnDecay != null)
                OnDecay(this, EventArgs.Empty);
        }
    }
}
