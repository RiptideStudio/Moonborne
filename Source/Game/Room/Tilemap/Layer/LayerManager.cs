
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            foreach (var layer in snapShot) 
            {
                if (!layer.Visible)
                    continue;

                layer.DrawBegin(spriteBatch);
                layer.Draw(spriteBatch);
                layer.DrawEnd(spriteBatch);
            }
        }

        /// <summary>
        /// Update all layers
        /// </summary>
        /// <param name="dt"></param>
        public static void Update(float dt)
        {
            foreach (var layer in Layers)
            {
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
        /// Initialize the layer manager with our core layers
        /// </summary>
        public static void Initialize()
        {
            // Static layers 
            AddLayer(new Layer(5, () => Camera.Transform, LayerType.Object, true), "Object");
            AddLayer(new Layer(4, () => Camera.Transform, LayerType.Object, true), "Tiles");
            AddLayer(new Layer(3, () => Camera.Transform, LayerType.Object, true), "Player");
            AddLayer(new Layer(9999, () => Camera.Transform, LayerType.Tile, true), "TileEditorWorld");
            AddLayer(new Layer(9999, () => WindowManager.Transform, LayerType.UI, true), "UI");
            AddLayer(new Layer(9999, () => WindowManager.Transform, LayerType.UI, true), "Dialogue");
            AddLayer(new Layer(1002, () => WindowManager.Transform, LayerType.UI, true), "Inventory");
            AddLayer(new Layer(1003, () => WindowManager.Transform, LayerType.UI, true), "RoomEditor");
        }
    }
}
