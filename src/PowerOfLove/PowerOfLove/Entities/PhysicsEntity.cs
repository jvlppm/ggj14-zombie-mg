﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGameLib.Core.Entities;
using MonoGameLib.Core.Extensions;
using MonoGameLib.Core.Sprites;
using Microsoft.Xna.Framework.Graphics;

namespace PowerOfLove.Entities
{
    public class PhysicsEntity : Entity
    {
        #region Attributes
        /// <summary>
        /// Angular forces that are constantly being applied to the body.
        /// </summary>
        float _angularAcceleration;
        /// <summary>
        /// Angular forces that will be applied only once to the body.
        /// These forces are not affected by game time.
        /// </summary>
        float _instantaneousAngularAcceleration;
        /// <summary>
        /// Forces/Accelerations that are constantly being applied to the body.
        /// </summary>
        Vector2 _acceleration;
        /// <summary>
        /// Forces that will be applied only once to the body.
        /// These forces are not affected by game time.
        /// </summary>
        Vector2 _instantaneousAcceleration;
        #endregion

        #region Properties
        /// <summary>
        /// Body momentum.
        /// </summary>
        public Vector2 Momentum { get; set; }

        /// <summary>
        /// Body angular momentum.
        /// </summary>
        public float AngularMomentum { get; set; }

        /// <summary>
        /// Movement Direction.
        /// </summary>
        public Vector2 Direction
        {
            get
            {
                return Vector2.Normalize(Momentum);
            }
        }

        /// <summary>
        /// Computes the actual speed of the entity.
        /// </summary>
        public float Speed
        {
            get
            {
                return Momentum.Length();
            }
        }

        /// <summary>
        /// Limit the entity maximum speed.
        /// </summary>
        /// <value>The max speed.</value>
        public float MaxSpeed { get; set; }

        /// <summary>
        /// Limit the entity maximum rotation speed.
        /// </summary>
        public float MaxRotationSpeed { get; set; }

        /// <summary>
        /// Body Mass.
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Does the entity rotate with the force?
        /// </summary>
        public bool RotateToMomentum { get; set; }

        /// <summary>
        /// Constant friction being applied on the body.
        /// </summary>
        public Vector2 Friction { get; set; }

        /// <summary>
        /// Constant friction being applied on the body's rotation.
        /// </summary>
        public float RotationFriction { get; set; }

        /// <summary>
        /// How much this entity can bounce on collision.
        /// </summary>
        public float Restitution { get; set; }
        #endregion Properties

        #region Constructors
        public PhysicsEntity()
        {
            MaxSpeed = int.MaxValue;
            Mass = 1;
        }

        public PhysicsEntity(Game game, string texturePath)
            : this()
        {
            if (texturePath != null)
            {
                Sprite = new Sprite(game.Content.Load<Texture2D>(texturePath), 1, 1);
                Sprite.AddAnimation( "default", 0, 1, TimeSpan.FromSeconds(1));
                Sprite.Origin = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y) / 2;
                Sprite.CurrentAnimation = "default";
            }
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Updates the Physics of the entity.
        /// </summary>
        /// <param name="gameTime">How much time have passed since the last update.</param>
        public override void Update(GameTime gameTime)
        {
            float secs = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 accelSecs = _acceleration * secs;

            Momentum += _instantaneousAcceleration;
            Momentum *= Vector2.One - Friction;
            var toMove = Momentum + accelSecs / 2;
            Position += toMove.LimitSize(MaxSpeed) * secs;

            Momentum += accelSecs;

            if (RotateToMomentum)
                Rotation = Momentum.GetAngle();
            else
            {
                var angularAccelSecs = _angularAcceleration * secs;

                AngularMomentum += _instantaneousAngularAcceleration;
                AngularMomentum *= 1 - RotationFriction;

                AngularMomentum = MathHelper.Clamp(AngularMomentum, -MaxRotationSpeed, MaxRotationSpeed);

                var toRotate = AngularMomentum + angularAccelSecs / 2;
                Rotation += MathHelper.Clamp(toRotate, -MaxRotationSpeed, MaxRotationSpeed) * secs;

                AngularMomentum += angularAccelSecs;

                _instantaneousAngularAcceleration = _angularAcceleration = 0;
            }

            _instantaneousAcceleration = _acceleration = Vector2.Zero;


            AngularMomentum = MathHelper.Clamp(AngularMomentum, -MaxRotationSpeed, MaxRotationSpeed);
            if (Speed > MaxSpeed)
                Momentum *= MaxSpeed / Speed;

            base.Update(gameTime);
        }

        /// <summary>
        /// Apply a rotation force to the body.
        /// </summary>
        /// <param name="force">How much force is being applied.</param>
        /// <param name="isAcceleration">True if the Mass should affect the object acceleration.</param>
        /// <param name="instantaneous">
        /// Indicates if this force will be applied only once to this body.
        /// Leave it to False if this force is being applied on every game loop.
        /// </param>
        public void ApplyRotation(float force, bool isAcceleration, bool instantaneous = false)
        {
#if DEBUG
            if (RotateToMomentum)
                throw new InvalidOperationException("This entity is set to RotateToMomentum and manual rotation is disabled.");
#endif
            float acceleration = isAcceleration ? force : force / Mass;

            if (instantaneous)
                _instantaneousAngularAcceleration += acceleration;
            else
                _angularAcceleration += acceleration;
        }

        /// <summary>
        /// Apply a force to the body.
        /// </summary>
        /// <param name="force">How much force to be applied.</param>
        /// <param name="instantaneous">
        /// Indicates if this force will be applied only once to this body.
        /// Leave it to False if this force is being applied on every game loop.
        /// </param>
        public void ApplyForce(Vector2 force, bool instantaneous = false)
        {
            if (instantaneous)
                _instantaneousAcceleration += force / Mass;
            else
                _acceleration += force / Mass;
        }

        /// <summary>
        /// Apply an acceleration to this body.
        /// The acceleration is applied regardless of the body's mass.
        /// </summary>
        /// <param name="acceleration">How much the entity should accelerate.</param>
        /// <param name="instantaneous">
        /// Indicates if this force will be applied only once to this body.
        /// Leave it to False if this force is being applied on every game loop.
        /// </param>
        public void ApplyAcceleration(Vector2 acceleration, bool instantaneous = false)
        {
            if (instantaneous)
                _instantaneousAcceleration += acceleration;
            else
                _acceleration += acceleration;
        }
        #endregion Methods
    }
}
