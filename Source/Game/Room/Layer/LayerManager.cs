
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions.Layers;
using Moonborne.Engine;
using Moonborne.Engine.Collision;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Inventory;
using Moonborne.Game.Objects;
using Moonborne.Graphics;
using Moonborne.Graphics.Camera;
using Moonborne.Graphics.Window;
using Moonborne.Input;
using Moonborne.UI.Dialogue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moonborne.Game.Room
{
    public static class LayerManager
    {
        public static Dictionary<string, Layer> Layers = new Dictionary<string, Layer>();
        public static Dictionary<string, Layer> LayersToAdd = new Dictionary<string, Layer>();
        public static List<GameObject> Objects = new List<GameObject>(); // Global list of all game objects

        /// <summary>
        /// Add a new layer to the list
        /// </summary>
        /// <param name="layer"></param>
        public static void AddLayer(Layer layer, string name)
        {
            if (!Layers.ContainsKey(name))
            {
                layer.Name = name;
                Layers.Add(name, layer);
                Sort();
            }
        }

        /// <summary>
        /// Returns a layer given a string name
        /// </summary>
        /// <param name="name"></param>
        public static Layer GetLayer(string name)
        {
            if (name == null)
                return null;

            return Layers.TryGetValue(name, out Layer layer) ? layer : null;
        }

        /// <summary>
        /// Returns a game object given an ID if it exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject GetInstance(int ID)
        {
            // Bad ID
            if (ID == -1)
                return null;

            GameObject obj = null;

            // Search the layers for the instance
            foreach (var layer in Layers)
            {
                obj = layer.Value.GetInstance(ID);

                if (obj != null)
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// Sort layers by depth
        /// </summary>
        public static void Sort()
        {
            Layers = Layers.Values.OrderBy(layer => layer.Depth).ToDictionary(layer => layer.Name);
        }

        /// <summary>
        /// Add a tilemap layer
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="tilemap"></param>
        /// <param name="name"></param>
        public static void AddTilemapLayer(Layer layer, Tilemap tilemap, string name)
        {
            if (!Layers.ContainsKey(name))
            {
                layer.Name = name;
                Layers.Add(name, layer);
                Layers = Layers.Values.OrderBy(layer => layer.Depth).ToDictionary(layer => layer.Name);
                layer.AddTilemap(tilemap);
                tilemap.Layer = layer;
            }
        }

        /// <summary>
        /// Add an object to the layer
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="layer"></param>
        public static void AddInstance(GameObject gameObject, string layerName)
        {
            // If there is no layer with this name, create one
            if (!Layers.ContainsKey(layerName))
            {
                Layer newLayer = new Layer(Layers.Count, () => Camera.TransformMatrix, LayerType.Object);
                AddLayer(newLayer,layerName);
            }

            // Add the instance to the layer
            Layer layer = Layers[layerName];

            if (layer != null)
            {
                layer.AddObject(gameObject);
                gameObject.Layer = layer;
                gameObject.Height = layer.Height;
            }
        }

        /// <summary>
        /// Remove an object from the game
        /// </summary>
        /// <param name="obj"></param>
        public static void RemoveInstance(GameObject obj)
        {
            foreach (var layer in Layers)
            {
                foreach (GameObject searchObj in layer.Value.Objects)
                {
                    if (searchObj == obj)
                    {
                        Objects.Remove(obj);
                        layer.Value.Objects.Remove(obj);
                        Console.WriteLine($"Deleted {obj.GetType().ToString()}");
                        obj = null;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Add a tile to be rendered
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="layerName"></param>
        public static void AddTilemap(Tilemap tileMap, string layerName)
        {
            Layer layer = Layers[layerName];

            if (layer != null)
            {
                layer.AddTilemap(tileMap);
                tileMap.Layer = layer;
            }
        }

        /// <summary>
        /// Render all layers
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            var snapShot = Layers.Values.ToList();

            // Render world objects
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.TransformMatrix);
            foreach (var layer in snapShot) 
            {
                // Don't draw invisible layers
                if (!layer.Visible)
                    continue;

                layer.Draw(spriteBatch);
            }

            RoomEditor.DrawGrid(spriteBatch);
            spriteBatch.End();

            // Render our UI
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, WindowManager.Transform);
            foreach (var layer in snapShot)
            {
                // Don't draw invisible layers
                if (!layer.Visible || layer.Type != LayerType.Object)
                    continue;

                layer.DrawUI(spriteBatch);
            }

            DialogueManager.DrawDialogueBox();
            InventoryManager.Draw(spriteBatch);
            RoomEditor.DrawEditor(spriteBatch);

            spriteBatch.End();
        }

        /// <summary>
        /// Update all layers
        /// </summary>
        /// <param name="dt"></param>
        public static void Update(float dt)
        {
            Sort();
            UpdateFrame(dt);
        }


        /// <summary>
        /// Executes just the update logic
        /// </summary>
        /// <param name="dt"></param>
        public static void UpdateFrame(float dt)
        {
            // Deal with collisions
            CollisionHandler.HandleCollisions(dt);

            // Deferral of adding and removing layers
            foreach (var layer in LayersToAdd)
            {
                if (Layers.ContainsKey(layer.Key))
                    continue;

                Layers.Add(layer.Key, layer.Value);
            }
            LayersToAdd.Clear();

            // Update all layers
            foreach (var layer in Layers)
            {
                if (!layer.Value.Visible)
                    continue;

                layer.Value.Update(dt);
            }
        }

        /// <summary>
        /// Removes all layers possible
        /// </summary>
        public static void Clear()
        {
            // Remove all existing layers
            foreach (var layer in Layers)
            {
                if (layer.Value.Locked)
                    continue;

                RemoveLayer(layer.Value);
            }

            // Clear all existing global objects
            List<GameObject> clearList = new List<GameObject>();

            foreach (GameObject obj in Objects)
            {
                if (obj.Layer.Locked)
                    continue;

                clearList.Add(obj);
            }

            Objects.RemoveAll(obj => clearList.Contains(obj));
        }

        /// <summary>
        /// Remove a layer if possible
        /// </summary>
        /// <param name="layer"></param>
        public static void RemoveLayer(Layer layer)
        {
            // Can't delete locked layers (static layers)
            if (layer == null || layer.Locked)
                return;

            Layers.Remove(layer.Name);
        }

        /// <summary>
        /// Normalize layer depth
        /// </summary>
        /// <param name="height"></param>
        /// <param name="minHeight"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static float NormalizeLayerDepth(int height, int minHeight, int maxHeight)
        {
            return 1f - ((float)(height - minHeight) / (maxHeight - minHeight));
        }

        /// <summary>
        /// Initialize the layer manager with our core layers
        /// </summary>
        public static void Initialize()
        {
            // Static layers 
            AddLayer(new Layer(3, () => Camera.TransformMatrix, LayerType.Object, true), "Player");
            AddLayer(new Layer(3, () => Camera.TransformMatrix, LayerType.Object, true), "Managers");
            AddLayer(new Layer(1000, () => Camera.TransformMatrix, LayerType.Object, true), "TileEditorWorld");
            AddLayer(new Layer(9999, () => WindowManager.Transform, LayerType.UI, true), "Dialogue");
            AddLayer(new Layer(1002, () => WindowManager.Transform, LayerType.UI, true), "Inventory");
            AddLayer(new Layer(1003, () => WindowManager.Transform, LayerType.UI, true), "RoomEditor");
        }
    }
}
