
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions.Layers;
using Moonborne.Engine;
using Moonborne.Engine.Collision;
using Moonborne.Game.Inventory;
using Moonborne.Game.Objects;
using Moonborne.Graphics.Camera;
using Moonborne.Graphics.Window;
using Moonborne.UI.Dialogue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moonborne.Game.Room
{
    public static class LayerManager
    {
        public static Dictionary<string, Layer> Layers = new Dictionary<string, Layer>();
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
                AddLayer(new Layer(Layers.Count, () => Camera.Transform, LayerType.Object), layerName);
            }

            // Add the instance to the layer
            Layer layer = Layers[layerName];

            if (layer != null)
            {
                layer.AddObject(gameObject);
            }

            Objects.Add(gameObject);
            gameObject.Layer = layer;
            gameObject.Height = layer.Height;
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
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.Transform);
            foreach (var layer in snapShot) 
            {
                // Don't draw invisible layers
                if (!layer.Visible)
                    continue;

                layer.Draw(spriteBatch);
            }

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
            spriteBatch.End();
        }

        /// <summary>
        /// Update all layers
        /// </summary>
        /// <param name="dt"></param>
        public static void Update(float dt)
        {
            Sort();

            // If we are in editor, we only want to draw our layers
            // This way we have no game logic execute
            if (GameManager.Paused)
                return;

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
            foreach (var layer in Layers)
            {
                RemoveLayer(layer.Value);
            }
        }

        /// <summary>
        /// Remove a layer if possible
        /// </summary>
        /// <param name="layer"></param>
        public static void RemoveLayer(Layer layer)
        {
            // Can't delete locked layers (static layers)
            if (layer.Locked)
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
            AddLayer(new Layer(5, () => Camera.Transform, LayerType.Object, true), "Object");
            AddLayer(new Layer(4, () => Camera.Transform, LayerType.Object, true), "Tiles");
            AddLayer(new Layer(3, () => Camera.Transform, LayerType.Object, true), "Player");
            AddLayer(new Layer(1000, () => Camera.Transform, LayerType.Tile, true), "TileEditorWorld");
            AddLayer(new Layer(9999, () => WindowManager.Transform, LayerType.UI, true), "Dialogue");
            AddLayer(new Layer(1002, () => WindowManager.Transform, LayerType.UI, true), "Inventory");
            AddLayer(new Layer(1003, () => WindowManager.Transform, LayerType.UI, true), "RoomEditor");
        }
    }
}
