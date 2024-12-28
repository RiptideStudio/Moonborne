using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Engine;
using Moonborne.Game.Room;
using Moonborne.Graphics;
using System;

namespace Moonborne.Game.Objects
{
    public class GameObjectData
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public string Name { get; set; }
        public string LayerName { get; set; }
    }

    /// <summary>
    /// Handle collision states
    /// </summary>
    public enum ECollisionState
    {
        Colliding,
        None
    }

    public abstract class GameObject
    {
        public Sprite SpriteIndex; // Sprite object to hold drawing data

        public Vector2 OldPosition; // Object position
        public Vector2 Position; // Object position
        public Vector2 DrawOffset = Vector2.Zero; // Offset position
        public Vector2 StartPosition = Vector2.Zero; // Offset position
        public Vector2 Scale { get; set; } = Vector2.One; // Object scale
        public float Rotation = 0; // Object rotation

        public bool Visible = true;
        public bool IsDestroyed = false; // If we are marked for destroy

        public float Speed = 0;
        public float LinearFriction = 8;
        public float AngularDampening = 0.25f;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public float AngularVelocity;
        public float MaxSpeed = 1000;

        public int Depth = 0;
        public int Frame = 0;
        public float FrameTime = 0;
        public int AnimationSpeed = 10;

        public Rectangle Hitbox = new Rectangle(0, 0, 16, 16);
        public bool Collideable = true;
        public bool IsStatic = false; // Static collisions don't get updated
        public ECollisionState CollisionState = ECollisionState.None;
        public Color Tint = Color.White;

        /// <summary>
        /// Base constructor
        /// </summary>
        public GameObject()
        {
            Create();
        }

        /// <summary>
        /// Called when an object is created
        /// </summary>
        public virtual void Create()
        {
            StartPosition = Position;
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

            // Clamp the magnitude of the velocity vector to the maximum speed
            float speed = Velocity.Length();
            if (speed > Speed)
            {
                Velocity *= Speed / speed;
            }

            ApplyFriction();

            // Clamp velocity to maximum speed
            Velocity.X = MathHelper.Clamp(Velocity.X, -MaxSpeed, MaxSpeed);
            Velocity.Y = MathHelper.Clamp(Velocity.Y, -MaxSpeed, MaxSpeed);

            // Add to position and rotation
            Position += Velocity * dt;
            Rotation += AngularVelocity * dt;

            // Update our hitbox if we are moving
            if (!IsStatic)
            {
                Hitbox.X = (int)Position.X-Hitbox.Width/2;
                Hitbox.Y = (int)Position.Y-Hitbox.Height/2;

                if (SpriteIndex != null)
                {
                    Hitbox.Width = SpriteIndex.FrameWidth;
                    Hitbox.Height = SpriteIndex.FrameHeight;
                }
            }

            // Update our animation
            if (SpriteIndex != null && AnimationSpeed > 0)
            {
                FrameTime += AnimationSpeed * dt;

                if (FrameTime > 1)
                {
                    FrameTime = 0;
                    Frame++;
                }

                if (Frame >= SpriteIndex.MaxFrames)
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

            // Draw the hitbox of our object
            if (GameManager.DebugMode)
            {
                SpriteManager.SetDrawAlpha(0.25f);
                SpriteManager.DrawRectangle(Hitbox.X, Hitbox.Y, Hitbox.Width, Hitbox.Height, Color.Red);
                SpriteManager.ResetDraw();
            }

            // If the sprite is valid, draw it
            if (SpriteIndex != null)
            {
                SpriteIndex.Color = Tint;
                SpriteIndex.Draw(spriteBatch, Frame, Position+DrawOffset, Scale, Rotation, SpriteIndex.Color);
            }
        }

        /// <summary>
        /// Draw event that draws using the Window's UI transform
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void DrawUI(SpriteBatch spriteBatch)
        {

        }

        /// <summary>
        /// Called when an object is destroyed
        /// </summary>
        public virtual void OnDestroy()
        {
        }

        /// <summary>
        /// While an object is colliding with another
        /// </summary>
        public virtual void OnCollision(GameObject other)
        {
            // Triggers a collision event once on enter
            if (CollisionState == ECollisionState.None)
            {
                OnCollisionStart(other);
            }
        }

        /// <summary>
        /// When an object enters another (single frame trigger)
        /// </summary>
        public virtual void OnCollisionStart(GameObject other)
        {
            CollisionState = ECollisionState.Colliding;
        }

        /// <summary>
        /// When we leave the collision
        /// </summary>
        public virtual void OnCollisionEnd()
        {
            CollisionState = ECollisionState.None;
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

        /// <summary>
        /// Applies friction to the object
        /// </summary>
        public void ApplyFriction()
        {
            // Stop the velocity entirely if it's close to zero (to avoid jittering)
            // Apply friction to velocity (subtractive)
            if (Velocity.X > 0)
            {
                Velocity.X -= LinearFriction;
                if (Velocity.X < 0)
                {
                    Velocity.X = 0;
                }
            }
            else if (Velocity.X < 0)
            {
                Velocity.X += LinearFriction;
                if (Velocity.X > 0)
                {
                    Velocity.X = 0;
                }
            }

            if (Velocity.Y > 0)
            {
                Velocity.Y -= LinearFriction;
                if (Velocity.Y < 0)
                {
                    Velocity.Y = 0;
                }
            }
            else if (Velocity.Y < 0)
            {
                Velocity.Y += LinearFriction;
                if (Velocity.Y > 0)
                {
                    Velocity.Y = 0;
                }
            }

            if (Math.Abs(Velocity.X) < 0.01f) Velocity.X = 0f;
            if (Math.Abs(Velocity.Y) < 0.01f) Velocity.Y = 0f;
        }
    }
}