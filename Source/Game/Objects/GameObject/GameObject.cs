using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.ImGui;
using Moonborne.Engine;
using Moonborne.Engine.Collision;
using Moonborne.Engine.Components;
using Moonborne.Engine.UI;
using Moonborne.Game.Room;
using Moonborne.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

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
        // Quick component pointers
        public Sprite SpriteIndex;
        public Transform Transform;
        public Physics Physics;

        public List<GameAction> Actions = new List<GameAction>();
        public List<GameAction> DeferredActions = new List<GameAction>();
        public List<GameAction> ActionsToDestroy = new List<GameAction>();
        public List<ObjectComponent> Components = new List<ObjectComponent>();

        public string Name;

        public int Height = 1; // Defines our heightmap
        public Vector2 StartPosition = Vector2.Zero; // Offset Transform.Position

        public bool IsDestroyed = false; // If we are marked for destroy

        public float Depth = 0;

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
        public int HitboxXOffset;
        public int HitboxYOffset;
        public int HitboxWidthOffset;
        public int HitboxHeightOffset;

        /// <summary>
        /// Adds a component to this object
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public void AddComponent(ObjectComponent component)
        {
            Components.Add(component);
        }

        /// <summary>
        /// Base constructor
        /// </summary>
        public GameObject()
        {
            AddComponent(SpriteIndex = new Sprite(this));
            AddComponent(Transform = new Transform(this));
            Name = GetType().BaseType.Name;
            Create();
        }

        /// <summary>
        /// Called when an object is created
        /// </summary>
        public virtual void Create()
        {
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
            // Update each component
            foreach (ObjectComponent component in Components)
            {
                component.Update(dt);
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
            // Draw each component
            foreach (ObjectComponent component in Components)
            {
                component.Draw(spriteBatch);
            }

            // Draw the hitbox of our object
            if (GameManager.DebugMode)
            {
                SpriteManager.SetDrawAlpha(0.25f);
                SpriteManager.DrawRectangle(Hitbox.X, Hitbox.Y, Hitbox.Width, Hitbox.Height, Color.Red);
                SpriteManager.SetDrawAlpha(1);
                SpriteManager.DrawText($"{GetType().Name}", Transform.Position, Transform.Scale, Transform.Rotation, Tint);
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