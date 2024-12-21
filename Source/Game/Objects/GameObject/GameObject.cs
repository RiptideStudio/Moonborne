using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Graphics;
using System;

namespace Moonborne.Game.Objects
{
    public abstract class GameObject
    {
        public Sprite SpriteIndex; // Sprite object to hold drawing data

        public Vector2 OldPosition; // Object position
        public Vector2 Position; // Object position
        public float x;
        public float y;
        public Vector2 Scale; // Object scale
        public float Rotation = 0; // Object rotation

        public bool Visible = true;
        public bool IsDestroyed = false; // If we are marked for destroy
        private bool IsDirty = true; // Whether to recalculate transform data

        public float Speed = 0;
        public float LinearFriction = 8;
        public float AngularDampening = 0.25f;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public float AngularVelocity;
        public float MaxSpeed = 100;

        public int Depth = 0;
        public int Frame = 0;
        public float FrameTime = 0;
        public int AnimationSpeed = 0;


        /// <summary>
        /// Constructor
        /// </summary>
        public GameObject()
        {
            GameObjectManager.Add(this);
            Create();
        }

        /// <summary>
        /// Called when an object is created
        /// </summary>
        public virtual void Create()
        {

        }

        /// <summary>
        /// Called when an object is updated
        /// </summary>
        /// <param name="dt"></param>
        public virtual void Update(float dt)
        {
            // Update our position and velocity
            OldPosition = Position;

            // Apply linear friction to acceleration (multiplicative decay for realism)
            Velocity += Acceleration;

            // Apply friction to velocity (subtractive)
            if (Velocity.X > 0)
                Velocity.X -= LinearFriction;
            else if (Velocity.X < 0)
                Velocity.X += LinearFriction;

            if (Velocity.Y > 0)
                Velocity.Y -= LinearFriction;
            else if (Velocity.Y < 0)
                Velocity.Y += LinearFriction;

            // Stop the velocity entirely if it's close to zero (to avoid jittering)
            if (Math.Abs(Velocity.X) < 0.01f) Velocity.X = 0f;
            if (Math.Abs(Velocity.Y) < 0.01f) Velocity.Y = 0f;

            // Clamp velocity to maximum speed
            Velocity.X = MathHelper.Clamp(Velocity.X, -MaxSpeed, MaxSpeed);
            Velocity.Y = MathHelper.Clamp(Velocity.Y, -MaxSpeed, MaxSpeed);

            // Add to position and rotation
            Position += Velocity * dt;
            Rotation += AngularVelocity * dt;

            // Update our animation
            if (AnimationSpeed > 0)
            {
                FrameTime += AnimationSpeed * dt;

                if (FrameTime > 1)
                {
                    FrameTime = 0;
                    Frame++;
                }

                if (Frame >= SpriteIndex.MaxFrames-1)
                {
                    Frame = 0;
                }
            }
        }

        /// <summary>
        /// Called when an object is drawn. Draws base sprite by default
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Can't draw if invisible
            if (!Visible)
            {
                return;
            }

            // If the sprite is valid, draw it
            if (SpriteIndex != null)
            {
                SpriteIndex.Draw(spriteBatch, Frame, Position, Scale, Rotation, SpriteIndex.Color);
            }
        }

        /// <summary>
        /// Called when an object is destroyed
        /// </summary>
        public virtual void OnDestroy()
        {

        }

        /// <summary>
        /// Marks an object for destruction
        /// </summary>
        public void Destroy()
        {
            IsDestroyed = true;
        }

        /// <summary>
        /// Draw a shadow on this object with a given size and position
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawShadow(float x, float y, float xRadius, float yRadius)
        {
            if (!Visible)
            {
                return;
            }

            // Draw an ellipse given a color
            SpriteManager.SetDrawAlpha(0.5f);
            SpriteManager.DrawEllipse(x+xRadius, y+yRadius, xRadius, yRadius, Color.Black);
            SpriteManager.ResetDraw();
        }
    }
}