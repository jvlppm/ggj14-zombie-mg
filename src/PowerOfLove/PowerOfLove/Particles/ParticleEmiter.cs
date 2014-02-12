using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoGameLib.Core.Extensions;
using PowerOfLove.Activities;
using MonoGameLib.Core.Sprites;

namespace MonoGameLib.Core.Particles
{
    class ParticleEmiter
    {
        #region Attributes
        private GamePlayScreen _level;
        private Sprite _particleSprite;
        private List<Particle> _particles;
        private float _sinceLastEmision;
        private Random _rng;
        private bool _decaying;

        /// <summary>
        /// The depth of a layer.
        /// By default, 0 represents the front layer and 1 represents a back layer.
        /// Use SpriteSortMode if you want sprites to be sorted during drawing.
        /// </summary>
        public float LayerDepth;
        #endregion

        #region Properties
        public float MillisecondsToEmit { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public float ParticleSpeed { get; set; }
        public float OpeningAngle { get; set; }
        public IEnumerable<ParticleState> ParticleStates { get; protected set; }
        public bool Enabled { get; set; }
        public float DecayTime { get; set; }
        public float Intensity { get; set; }
        public Vector2 Momentum { get; set; }
        #endregion Properties

        #region Delegates
        public event EventHandler OnDecay;
        #endregion Delegates

        #region Constructors
        public ParticleEmiter(Game game, GamePlayScreen level, string spritePath, IEnumerable<ParticleState> particleStates)
            : this(game, level, LoadSprite(game, spritePath), particleStates)
        {
        }

        public ParticleEmiter(Game game, GamePlayScreen level, Sprite sprite, IEnumerable<ParticleState> particleStates)
        {
            LayerDepth = 1;
            _level = level;
            Intensity = 1f;

            _particleSprite = sprite;
            _rng = new Random();
            ParticleStates = particleStates;
            Enabled = true;
            _decaying = false;

            _particles = new List<Particle>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Updates particle emiter.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        public void Update(GameTime gameTime)
        {
            if (DecayTime > 0f)
            {
                DecayTime -= gameTime.ElapsedGameTime.Milliseconds;

                if (DecayTime <= 0f)
                {
                    Enabled = false;
                    _decaying = true;
                }
            }

            if (Enabled)
            {
                _sinceLastEmision += gameTime.ElapsedGameTime.Milliseconds;
                var toEmit = MillisecondsToEmit / Intensity;

                while (_sinceLastEmision >= toEmit)
                {
                    float angle = (float)_rng.NextDouble() * OpeningAngle - OpeningAngle / 2;
                    _sinceLastEmision -= toEmit;
                    var particle = new Particle(_level, _particleSprite, Position, ParticleStates)
                    {
                        LayerDepth = LayerDepth,
                        Opacity = Intensity,
                        Speed = ParticleSpeed,
                        Direction = Direction.Rotate(angle)
                    };
                    _particles.Add(particle);
                    _level.AddEntity(particle);
                    particle.OnDecay += Particle_OnDecay;
                }
            }
        }

        void Particle_OnDecay(object sender, EventArgs e)
        {
            _particles.Remove((Particle)sender);
            if (_decaying && _particles.Count == 0 && OnDecay != null)
            {
                OnDecay(this, EventArgs.Empty);
            }
        }

        static Sprite LoadSprite(Game game, string path)
        {
            var texture = game.Content.Load<Texture2D>(path);
            var sprite = new Sprite(texture, 1, 1)
            {
                Origin = new Vector2(texture.Width / 2, texture.Height / 2)
            };
            sprite.AddAnimation("default", new[] { 0 }, TimeSpan.FromSeconds(1), true);
            return sprite;
        }
        #endregion Methods
    }
}
