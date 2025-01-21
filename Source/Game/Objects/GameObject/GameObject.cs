using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.ImGui;
using Moonborne.Engine;
using Moonborne.Engine.Collision;
using Moonborne.Engine.UI;
using Moonborne.Game.Room;
using Moonborne.Graphics;
using System;
using System.Collections.Generic;

namespace Moonborne.Game.Objects
{
    public class GameObjectData
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public string Name { get; set; }
        public string LayerName { get; set; }
        public int Depth { get; set; }
        public List<VariableData> Properties { get; set; }
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
        public List<GameAction> Actions = new List<GameAction>();
        public List<GameAction> DeferredActions = new List<GameAction>();
        public List<GameAction> ActionsToDestroy = new List<GameAction>();

        public int AnimationSpeed { get; set; } = 10;
        public Vector2 Scale { get; set; } = Vector2.One; // Object scale
        public float MaxSpeed { get; set; } = 1000;
        public bool Visible { get; set; } = true;
        public float Speed { get; set; } = 0;
        public float LinearFriction { get; set; } = 8;
        public float Alpha = 0f;

        public int Height = 1; // Defines our heightmap
        public Vector2 Velocity;
        public Vector2 OldPosition; // Object position
        public Vector2 Position; // Object position
        public Vector2 DrawOffset = Vector2.Zero; // Offset position
        public Vector2 StartPosition = Vector2.Zero; // Offset position
        public float Rotation = 0; // Object rotation

        public bool IsDestroyed = false; // If we are marked for destroy

        public float AngularDampening = 0.25f;
        public Vector2 Acceleration;
        public float AngularVelocity;

        public float Depth = 0;
        public int Frame = 0;
        public float FrameTime = 0;

        public Rectangle Hitbox = new Rectangle(0, 0, -1, -1);
        public bool Collideable = true;
        public bool IsStatic = false; // Static collisions don't get updated
        public ECollisionState CollisionState = ECollisionState.None;
        public Color Tint = Color.White;
        public Layer Layer; // The layer this object is on
        public Layer PreviousLayer; // The layer this object is on
        public int PreviousTileX = 0;
        public int PreviousTileY = 0;
        public List<Tile> TileList = new List<Tile>();

        public bool NeedsLayerSort = false; // Used as a flag when changing layers
        public bool Colliding = false;
        public bool IsDirty = true;
        private int CurrentLayer;
        private int Haxis = 1;
        private int Vaxis = 1;
        private int HitboxXOffset;
        private int HitboxYOffset;
        private int HitboxWidthOffset;
        private int HitboxHeightOffset;
        public bool VisibleInGame = true;

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
        /// Create event called after initialization
        /// </summary>
        public virtual void CreateLater()
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

            if (Velocity.X < 0)
                Haxis = -1;
            else if (Velocity.X > 0)
                Haxis = 1;

            if (Velocity.Y < 0)
                Vaxis = -1;
            else if (Velocity.Y > 0)
                Vaxis = 1;

            Rectangle horizontalRect = new Rectangle((int)Position.X-Hitbox.Width/2+Haxis+(int)(Velocity.X*dt), (int)Position.Y-Hitbox.Height/2,Hitbox.Width,Hitbox.Height);
            Rectangle verticalRect = new Rectangle((int)Position.X-Hitbox.Width/2,(int)Position.Y-Hitbox.Height/2+ Vaxis + (int)(Velocity.Y * dt), Hitbox.Width,Hitbox.Height);

            if (CollisionHandler.CollidingWithTile(Position.X, Position.Y, horizontalRect))
            {
                Velocity.X = 0;
                Position.X = Hitbox.Width/2+ (int)(Position.X / Hitbox.Width)* Hitbox.Width;
            }
            if (CollisionHandler.CollidingWithTile(Position.X, Position.Y, verticalRect))
            {
                Velocity.Y = 0;
                Position.Y = Hitbox.Height / 2 + (int)(Position.Y / Hitbox.Height) * Hitbox.Height;
            }

            Position += Velocity * dt;
            Rotation += AngularVelocity * dt;

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

            // Update game actions here
            foreach (GameAction action in Actions)
            {
                action.Update(dt);
            }

            // Some actions can't overlap, so we defer them
            foreach (GameAction action in DeferredActions)
            {
                if (Actions.Count == 0)
                {
                    Actions.Add(action);
                    ActionsToDestroy.Add(action);
                }
            }

            // Destroy actions
            foreach(GameAction action in ActionsToDestroy)
            {
                if (Actions.Contains(action))
                {
                    Actions.Remove(action);
                }
                if (DeferredActions.Contains(action))
                {
                    DeferredActions.Remove(action);
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

            // Can't draw if not visible in game
            if (!VisibleInGame && !GameManager.Paused && !GameManager.DebugMode)
            {
                return;
            }

            // Update our hitbox if we are moving
            Hitbox.X = HitboxXOffset+(int)Position.X - Hitbox.Width / 2;
            Hitbox.Y = HitboxYOffset+(int)Position.Y - Hitbox.Height / 2;

            if (SpriteIndex != null)
            {
                Hitbox.Width = SpriteIndex.FrameWidth-HitboxWidthOffset;
                Hitbox.Height = SpriteIndex.FrameHeight-HitboxHeightOffset;
            }

            // Draw the hitbox of our object
            if (GameManager.DebugMode)
            {
                SpriteManager.SetDrawAlpha(0.25f);
                SpriteManager.DrawRectangle(Hitbox.X, Hitbox.Y, Hitbox.Width, Hitbox.Height, Color.Red);
                SpriteManager.SetDrawAlpha(1);
                SpriteManager.DrawText($"{GetType().Name}", Position, Scale, Rotation, Tint);
            }

            // Resort an object based on its layer's depth and Y position
            if (NeedsLayerSort && SpriteIndex != null)
            {
                Depth = LayerManager.NormalizeLayerDepth((int)Position.Y, 1, 99999999) - 0.0001f;
                Depth = Math.Clamp(Depth, 0, 1);
                SpriteIndex.LayerDepth = Depth;
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

        /// <summary>
        /// Add a new action to this object
        /// </summary>
        /// <param name="moveAction"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void AddAction(GameAction action, bool deferred = false, bool overwriteExisting = false)
        {
            action.Parent = this;

            if (overwriteExisting)
            {
                Actions.Clear();
            }

            if (deferred)
            {
                DeferredActions.Add(action);
            }
            else
            {
                Actions.Add(action);
            }
        }
    }
}