
using Microsoft.Xna.Framework;
using Moonborne.Engine.Collision;
using Moonborne.Game.Objects;
using System;
using static Moonborne.Graphics.Sprite;

namespace Moonborne.Engine.Components
{
    public class Physics : ObjectComponent
    {
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; } = Vector2.One;

        /// <summary>
        /// Force of gravity
        /// </summary>
        public float Gravity { get; set; } = 0f; 
        public float LinearFriction { get; set; } = 8;
        public float MaxSpeed { get; set; } = 1000;
        public float Speed { get; set; } = 0;

        // Keep track of the axes we are moving on
        private int Haxis = 1;
        private int Vaxis = 1;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="parent"></param>
        public Physics(GameObject parent) : base(parent)
        {

        }

        /// <summary>
        /// Initialize physics
        /// </summary>
        public override void Create()
        {
            Name = "Physics";
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

            // Clamp the magnitude of the velocity vector to the maximum speed
            float speed = Velocity.Length();
            if (speed > Speed)
            {
                Velocity *= Speed / speed;
            }

            ApplyFriction();

            // Clamp velocity to maximum speed
            Vector2 velocity = Velocity;
            velocity.Y += Gravity;
            velocity.X = MathHelper.Clamp(velocity.X, -MaxSpeed, MaxSpeed);
            velocity.Y = MathHelper.Clamp(velocity.Y, -MaxSpeed, MaxSpeed);

            if (velocity.X < 0)
                Haxis = -1;
            else if (velocity.X > 0)
                Haxis = 1;

            if (velocity.Y < 0)
                Vaxis = -1;
            else if (velocity.Y > 0)
                Vaxis = 1;

            Rectangle horizontalRect = new Rectangle((int)Parent.Transform.Position.X - Parent.Hitbox.Width / 2 + Haxis + (int)(velocity.X * dt), (int)Parent.Transform.Position.Y - Parent.Hitbox.Height / 2, Parent.Hitbox.Width, Parent.Hitbox.Height);
            Rectangle verticalRect = new Rectangle((int)Parent.Transform.Position.X - Parent.Hitbox.Width / 2, (int)Parent.Transform.Position.Y - Parent.Hitbox.Height / 2 + Vaxis + (int)(velocity.Y * dt), Parent.Hitbox.Width, Parent.Hitbox.Height);
            Vector2 finalPos = Parent.Transform.Position;

            if (CollisionHandler.CollidingWithTile(Parent.Transform.Position.X, Parent.Transform.Position.Y, horizontalRect))
            {
                velocity.X = 0;
                finalPos.X = Parent.Hitbox.Width / 2 + (int)(Parent.Transform.Position.X / Parent.Hitbox.Width) * Parent.Hitbox.Width;
            }
            if (CollisionHandler.CollidingWithTile(Parent.Transform.Position.X, Parent.Transform.Position.Y, verticalRect))
            {
                velocity.Y = 0;
                finalPos.Y = Parent.Hitbox.Height / 2 + (int)(Parent.Transform.Position.Y / Parent.Hitbox.Height) * Parent.Hitbox.Height;
            }

            Velocity = velocity;

            Parent.Transform.Position = finalPos;
            Parent.Transform.Position += Velocity * dt;
        }

        /// <summary>
        /// Applies friction to the object
        /// </summary>
        public void ApplyFriction()
        {
            Vector2 velocity = Velocity;

            // Stop the velocity entirely if it's close to zero (to avoid jittering)
            // Apply friction to velocity (subtractive)
            if (velocity.X > 0)
            {
                velocity.X -= LinearFriction;
                if (velocity.X < 0)
                {
                    velocity.X = 0;
                }
            }
            else if (velocity.X < 0)
            {
                velocity.X += LinearFriction;
                if (velocity.X > 0)
                {
                    velocity.X = 0;
                }
            }

            if (velocity.Y > 0)
            {
                velocity.Y -= LinearFriction;
                if (velocity.Y < 0)
                {
                    velocity.Y = 0;
                }
            }
            else if (velocity.Y < 0)
            {
                velocity.Y += LinearFriction;
                if (velocity.Y > 0)
                {
                    velocity.Y = 0;
                }
            }

            if (Math.Abs(velocity.X) < 0.01f) velocity.X = 0f;
            if (Math.Abs(velocity.Y) < 0.01f) velocity.Y = 0f;

            Velocity = velocity;
        }

    }
}
