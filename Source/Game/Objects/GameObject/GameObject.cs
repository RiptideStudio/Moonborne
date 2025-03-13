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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Moonborne.Game.Objects
{
    public class GameObjectData
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public string Name { get; set; }
        public string LayerName { get; set; }
        public int Depth { get; set; }
        public int InstanceID { get; set; }
        public List<VariableData> Properties { get; set; }
    }

    public class GameObjectWorldData
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string LayerName { get; set; }
        public int InstanceID { get; set; }
        public List<string> DialogueNames { get; set; } = new List<string>();
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
        /// <summary>
        /// This represents a unique identifier for each object
        /// </summary>
        internal int InstanceID;

        /// <summary>
        /// The name used in the display of this object
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        public Sprite SpriteIndex;
        public Transform Transform;
        public Physics Physics;
        public string Name;

        public Rectangle Hitbox = new Rectangle(0, 0, -1, -1);
        public bool Collideable = true;
        public bool IsStatic = false; // Static collisions don't get updated
        protected ECollisionState CollisionState = ECollisionState.None;
        public Color Tint = Color.White;
        public int HitboxXOffset;
        public int HitboxYOffset;
        public int HitboxWidthOffset;
        public int HitboxHeightOffset;

        internal List<ObjectComponent> Components = new List<ObjectComponent>();
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


        /// <summary>
        /// Loads prefab data into a specified object
        /// </summary>
        /// <param name="prefab"></param>
        public static void LoadData(GameObject gameObject, GameObjectData data)
        {
            gameObject.Transform.Position = new Vector2(data.PositionX, data.PositionY);
            gameObject.InstanceID = data.InstanceID;
            gameObject.DisplayName = data.Name;

            foreach (var property in data.Properties)
            {
                Room.Room.LoadProperty(gameObject, property);
            }
        }

        /// <summary>
        /// Loads gameobject data using a filepath instead
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        public static void LoadData(GameObject gameObject, string filePath, string name)
        {
            if (Directory.Exists(filePath))
            {
                string file = filePath + "/" + name+".json";

                if (File.Exists(file))
                {
                    string json = File.ReadAllText(file);
                    var gameObjectData = JsonSerializer.Deserialize<GameObjectData>(json);

                    LoadData(gameObject, gameObjectData);
                }
            }
        }

        /// <summary>
        /// Saves a prefab's data given its name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filePath"></param>
        public static GameObjectData SaveData(GameObject prefab)
        {
            // Don't save null prefabs
            if (prefab == null)
                return null;

            // Serialize all of our data
            PropertyInfo[] properties = prefab.GetType().GetProperties();
            var objectProperties = new List<VariableData>();

            // Save object properties
            foreach (var property in properties)
            {
                VariableData variableData = new VariableData();
                variableData.Name = property.Name;
                variableData.Value = property.GetValue(prefab, null);
                variableData.Type = property.GetType().ToString();

                objectProperties.Add(variableData);
            }

            // Save component properties
            foreach (ObjectComponent comp in prefab.Components)
            {
                foreach (var property in comp.GetType().GetProperties())
                {
                    VariableData variableData = new VariableData();
                    variableData.Name = property.Name;
                    variableData.Type = property.GetType().ToString();
                    var propertyValue = property.GetValue(comp);

                    // Vectors
                    Type propertyType = propertyValue.GetType();
                    if (propertyType == typeof(Vector2))
                    {
                        Vector2 vectorValue = (Vector2)propertyValue;

                        variableData.X = vectorValue.X;
                        variableData.Y = vectorValue.Y;

                        variableData.Type = "Vector";
                        objectProperties.Add(variableData);
                    }
                    // Save sprite texture data
                    else if (propertyType == typeof(SpriteTexture))
                    {
                        SpriteTexture val = (SpriteTexture)propertyValue;

                        variableData.Type = "SpriteTexture";
                        variableData.Value = val.Name;
                        objectProperties.Add(variableData);
                    }
                    else
                    // Normal types
                    {
                        variableData.Value = propertyValue;
                        objectProperties.Add(variableData);
                    }
                }
            }

            // Construct our game object data
            GameObjectData gameObjectData;

            // If we are a prefab
            gameObjectData = new GameObjectData
            {
                PositionX = 0,
                PositionY = 0,
                Name = prefab.DisplayName,
                Depth = (int)prefab.Depth,
                LayerName = "None",
                Properties = objectProperties,
                InstanceID = prefab.InstanceID
            };

            return gameObjectData;
        }

        /// <summary>
        /// Given gameobject data, save it to a json file
        /// </summary>
        /// <param name="data"></param>
        public static void SaveObjectDataToFile(GameObjectData data, string directory)
        {
            // Write to the file
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            // Create a new file
            string filePath = $@"{directory}/{data.Name}.json";

            // If the directory doesn't exist, throw an error
            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"Failed to save object to {filePath}");
                return;
            }

            // Attempt to the open the file
            File.WriteAllText(filePath, json);
        }
    }
}