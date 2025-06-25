using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGui;
using Moonborne.Engine;
using Moonborne.Engine.Components;
using Moonborne.Engine.UI;
using Moonborne.Game.Objects.Prefabs;
using Moonborne.Game.Room;
using Moonborne.Graphics;
using Moonborne.UI.Dialogue;
using Moonborne.Utils.Math;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Moonborne.Game.Objects
{ 
    /// <summary>
    /// Handle collision states
    /// </summary>
    public enum ECollisionState
    {
        Colliding,
        None
    }

    [Serializable]
    public abstract class GameObject : BaseGameBehavior, IComponentContainer
    {
        public List<ObjectComponent> Components { get; private set; } = new List<ObjectComponent>();

        /// <summary>
        /// This represents a unique identifier for each object
        /// </summary>
        internal int InstanceID;

        /// <summary>
        /// The name used in the display of this object
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        internal Sprite SpriteIndex;
        internal Transform Transform;
        internal Physics Physics;
        public string Name;

        public Rectangle Hitbox = new Rectangle(0, 0, -1, -1);
        public bool Collideable = true;
        public bool IsStatic = false; // Static collisions don't get updated
        protected ECollisionState CollisionState = ECollisionState.None;
        internal int HitboxXOffset;
        internal int HitboxYOffset;
        internal int HitboxWidthOffset;
        internal int HitboxHeightOffset;

        internal List<GameAction> Actions = new List<GameAction>();
        internal List<GameAction> DeferredActions = new List<GameAction>();
        internal List<GameAction> ActionsToDestroy = new List<GameAction>();
        internal IntPtr Icon;
        internal int Height = 1; // Defines our heightmap
        internal Vector2 StartPosition = Vector2.Zero; // Offset Transform.Position
        internal bool IsDestroyed = false; // If we are marked for destroy
        internal float Depth = 0;
        internal Layer Layer; // The layer this object is on
        internal Layer PreviousLayer; // The layer this object is on
        internal int PreviousTileX = 0;
        internal int PreviousTileY = 0;
        internal List<Tile> TileList = new List<Tile>();
        internal bool NeedsLayerSort = false; // Used as a flag when changing layers
        internal bool Colliding = false;
        internal bool IsDirty = true;

        /// <summary>
        /// Base constructor
        /// </summary>
        public GameObject()
        {
            // Give all objects the name of their base type
            Name = GetType().BaseType.Name;

            Create();
        }

        /// <summary>
        /// Builds any attached assets that need to be updated
        /// </summary>
        public override void OnBuild()
        {
            foreach (ObjectComponent component in Components)
            {
                component.OnBuild();
            }
        }

        /// <summary>
        /// Adds a component to this object
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public void AddComponent(ObjectComponent component)
        {
            component.Parent = this;
            component.Create();
            Components.Add(component);
        }

        /// <summary>
        /// Remove a component
        /// </summary>
        /// <param name="component"></param>
        public void RemoveComponent(ObjectComponent component)
        {
            Components.Remove(component);
        }

        /// <summary>
        /// Get a component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : ObjectComponent
        {
            return Components.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Called when an object is created
        /// </summary>
        public override void Create()
        {
            InstanceID = MoonMath.RandomRange(0, 65535);
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
        public override void Update(float dt)
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
        /// Get the tilelist of colliding tiles
        /// </summary>
        /// <returns></returns>
        public List<Tile> GetTiles()
        {
            return TileList;
        }

        /// <summary>
        /// Resort the object
        /// </summary>
        public void Resort()
        {
            NeedsLayerSort = true;
        }

        /// <summary>
        /// Called when an object is drawn. Draws base sprite by default
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw each component
            foreach (ObjectComponent component in Components)
            {
                // Do not draw components if they are invisible
                if (!component.Visible || (!component.VisibleInGame && !GameManager.Paused))
                {
                    continue;
                }

                component.Draw(spriteBatch);
            }

            // Draw the hitbox of our object
            if (GameManager.DebugMode)
            {
                Transform transform = GetComponent<Transform>();
                SpriteManager.SetDrawAlpha(0.25f);
                SpriteManager.DrawRectangle(Hitbox.X, Hitbox.Y, Hitbox.Width, Hitbox.Height, Color.Red);
                SpriteManager.SetDrawAlpha(1);
                SpriteManager.DrawText($"{GetType().Name}", transform.Position, transform.Scale, transform.Rotation, Color.White);
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
        public override void Destroy()
        {
            IsDestroyed = true;
        }

        /// <summary>
        /// Call all begin play for components and self
        /// </summary>
        public override void OnBeginPlay()
        {
            foreach (ObjectComponent component in Components)
            {
                component.OnBeginPlay();
            }
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

        /// <summary>
        /// Returns an object's id if it exists
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns>ID on success, -1 on fail</returns>
        public static int GetID(object gameObject)
        {
            if (gameObject == null || !(gameObject is GameObject))
            {
                return -1;
            }

            // This is a game object and has a valid ID
            return ((GameObject)(gameObject)).InstanceID;
        }

       
    }
}