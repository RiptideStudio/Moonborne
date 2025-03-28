﻿
using Microsoft.Xna.Framework;
using Moonborne.Engine.Collision;
using Moonborne.Game.Objects;
using Moonborne.Utils.Math;
using System;
using static Moonborne.Graphics.Sprite;

namespace Moonborne.Engine.Components
{
    public class Physics : ObjectComponent
    {
        internal override string Name => "Physics";
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration = Vector2.One;

        /// <summary>
        /// Force of gravity
        /// </summary>
        public float Gravity = 0f; 
        public float LinearFriction = 0.3f;
        public float MaxSpeed = 1000;
        public float Speed = 50;

        // Keep track of the axes we are moving on
        private int Haxis = 1;
        private int Vaxis = 1;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="parent"></param>
        public Physics() : base()
        {
            Description = "Stores an object's physics data (velocity, acceleration, friction)";
        }

        /// <summary>
        /// Set the velocity's X
        /// </summary>
        /// <param name="x"></param>
        public void SetVelocityX(float x)
        {
            Velocity = new Vector2(x, Velocity.Y);
        }

        /// <summary>
        /// Set the velocity's X
        /// </summary>
        /// <param name="x"></param>
        public void SetVelocityY(float y)
        {
            Velocity = new Vector2(Velocity.X, y);
        }

        /// <summary>
        /// Update our position and old position
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            // Apply linear friction to acceleration (multiplicative decay for realism)
            Velocity += Acceleration;

            // Clamp velocity to maximum speed
            Vector2 velocity = Velocity;
            velocity.Y += Gravity;
            velocity.X = MathHelper.Clamp(velocity.X, -MaxSpeed, MaxSpeed);
            velocity.Y = MathHelper.Clamp(velocity.Y, -MaxSpeed, MaxSpeed);

            Transform Transform = Parent.GetComponent<Transform>();

            Rectangle horizontalRect = new Rectangle((int)Transform.Position.X - Parent.Hitbox.Width / 2 + Haxis + (int)(velocity.X * dt), (int)Transform.Position.Y - Parent.Hitbox.Height / 2, Parent.Hitbox.Width, Parent.Hitbox.Height);
            Rectangle verticalRect = new Rectangle((int)Transform.Position.X - Parent.Hitbox.Width / 2, (int)Transform.Position.Y - Parent.Hitbox.Height / 2 + Vaxis + (int)(velocity.Y * dt), Parent.Hitbox.Width, Parent.Hitbox.Height);
            Vector2 finalPos = Transform.Position;

            if (CollisionHandler.CollidingWithTile(Transform.Position.X, Transform.Position.Y, horizontalRect))
            {
                velocity.X = 0;
                finalPos.X = Parent.Hitbox.Width / 2 + (int)(Transform.Position.X / Parent.Hitbox.Width) * Parent.Hitbox.Width;
            }
            if (CollisionHandler.CollidingWithTile(Transform.Position.X, Transform.Position.Y, verticalRect))
            {
                velocity.Y = 0;
                finalPos.Y = Parent.Hitbox.Height / 2 + (int)(Transform.Position.Y / Parent.Hitbox.Height) * Parent.Hitbox.Height;
            }

            Velocity = velocity;

            Transform.Position = finalPos;
            Transform.Position += Velocity * dt;

            ApplyFriction();
        }

        /// <summary>
        /// Applies friction to the object
        /// </summary>
        public void ApplyFriction()
        {
            // Apply friction to velocity (subtractive)
            Velocity = MoonMath.Lerp(Velocity, Vector2.Zero, LinearFriction);
            Vector2 velocity = Velocity;

            // Stop the velocity entirely if it's close to zero (to avoid jittering)
            if (Math.Abs(velocity.X) < 0.01f) velocity.X = 0f;
            if (Math.Abs(velocity.Y) < 0.01f) velocity.Y = 0f;

            Velocity = velocity;
        }

    }
}
