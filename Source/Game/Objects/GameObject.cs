using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Graphics;
using Moonborne.Game.Components;
using System.Collections.Generic;

namespace Moonborne.Game.Objects
{
    public abstract class GameObject
    {
        public Sprite sprite; // Sprite object to hold drawing data

        public Vector2 OldPosition; // Object position
        public Vector2 Position; // Object position
        public Vector2 Scale; // Object scale
        public float Rotation = 0; // Object rotation
        public bool IsDestroyed = false; // If we are marked for destroy
        private bool IsDirty = true; // Whether to recalculate transform data

        public float Speed = 0;
        public float LinearFriction = 0.2f;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        public int Depth = 0;
        public int Frame = 0;
        public float FrameTime = 0;
        public int AnimationSpeed = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public GameObject()
        {
            sprite = new Sprite(null,this);
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
            OldPosition = Position;
            Position += Velocity * dt;
            Velocity.X = MathHelper.Lerp(Velocity.X,0.0f,LinearFriction);
            Velocity.Y = MathHelper.Lerp(Velocity.Y,0.0f, LinearFriction);

            if (AnimationSpeed > 0)
            {
                FrameTime += AnimationSpeed * dt;

                if (FrameTime > 1)
                {
                    FrameTime = 0;
                    Frame++;
                }

                if (Frame >= sprite.MaxFrames)
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
            sprite.Draw(spriteBatch,Position);
        }

        /// <summary>
        /// Called when an object is destroyed
        /// </summary>
        public virtual void Destroy()
        {

        }
    }
}