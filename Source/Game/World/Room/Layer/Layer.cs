
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Engine;
using Moonborne.Engine.Collision;
using Moonborne.Engine.UI;
using Moonborne.Game.Inventory;
using Moonborne.Game.Objects;
using Moonborne.Graphics;
using Moonborne.Graphics.Window;
using Moonborne.UI.Dialogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Moonborne.Game.Room
{
    /// <summary>
    /// Define the type of layer this is (object, tile)
    /// </summary>
    public enum LayerType
    {
        Object,
        Tile,
        UI
    }

    public class Layer
    {
        private readonly Func<Matrix> GetTransform;

        public bool Locked = false;
        public bool Collideable = false;
        public bool Visible = true;
        public int Depth = 0;
        public string Name = string.Empty;
        public LayerType Type = LayerType.Object;
        public int Height = 1; // Defines the heightmap of this layer
        public List<Tilemap> Tilemaps = new List<Tilemap>(); // Tiles on this layer
        public List<GameObject> Objects = new List<GameObject>(); // Objects on this layer

        internal List<GameObject> ObjectsToCreate = new List<GameObject>();
        internal Matrix Transform => GetTransform(); // The matrix used to transform the layer
        internal SpriteSortMode SortMode = SpriteSortMode.Deferred; // The way sprites are sorted
        internal BlendState BlendState = BlendState.NonPremultiplied; // Type of blend state used
        internal SamplerState SamplerState = SamplerState.PointClamp; // Sampler state used (usually pixel)

        /// <summary>
        /// Construct a new layer
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="matrix"></param>
        /// <param name="layerType"></param>
        public Layer(int depth, Func<Matrix> matrix, LayerType layerType, bool locked = false)
        {
            Depth = depth;
            GetTransform = matrix;
            Type = layerType;
            Locked = locked;
        }

        public Layer()
        {
        }

        /// <summary>
        /// Gets a game object given its ID
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public GameObject GetInstance(int instanceID)
        {
            foreach (GameObject obj in Objects)
            {
                if (obj.InstanceID == instanceID)
                {
                    return obj;
                }
            }

            foreach (GameObject obj in ObjectsToCreate)
            {
                if (obj.InstanceID == instanceID)
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// Add an object to the layer
        /// </summary>
        /// <param name="obj"></param>
        public void AddObject(GameObject obj)
        {
            ObjectsToCreate.Add(obj);
        }        
        
        /// <summary>
        /// Add a tile to be rendered
        /// </summary>
        /// <param name="obj"></param>
        public void AddTilemap(Tilemap tileMap)
        {
            Tilemaps.Add(tileMap);
        }

        /// <summary>
        /// Draw a layer's settings
        /// </summary>
        public void DrawSettings()
        {

            // Type specific flags
            switch (Type)
            {

                case LayerType.Tile:

                    // Select tileset texture
                    var textures = SpriteManager.textures.ToList();

                    if (ImGui.TreeNodeEx("Texture"))
                    {
                        foreach (var tex in textures)
                        {
                            if (ImGui.Selectable(tex.Key))
                            {
                                Tilemap tilemap = Tilemaps[0];
                                tilemap.SetTexture(tex.Key);
                            }
                        }
                        ImGui.TreePop();
                    }
                    break;
            }

            // End global layer flags
            if (ImGui.Button("Delete"))
            {
                LayerManager.RemoveLayer(this);
            }
        }

        /// <summary>
        /// Begin drawing
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawBegin(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SortMode, BlendState, SamplerState, transformMatrix: Transform);
        }

        /// <summary>
        /// Render anything on the layer
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Render each tilemap
            foreach (var tileMap in Tilemaps)
            {
                tileMap.Draw(spriteBatch);
            }

            // Render each object
            foreach (var obj in Objects)
            {
                // No drawing if visibility marks
                if (obj.SpriteIndex != null)
                {
                    if (!obj.SpriteIndex.Visible)
                        continue;

                    if (!obj.SpriteIndex.VisibleInGame && !GameManager.Paused)
                        continue;
                }

                obj.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Call UI draw calls from objects
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawUI(SpriteBatch spriteBatch)
        {
            // Only objects have UI draw methods
            if (Objects.Count == 0)
                return;

            foreach (var obj in Objects)
            {
                // Execute object's world draw event
                obj.DrawUI(spriteBatch);
            }
        }

        /// <summary>
        /// End drawing spritebatch
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawEnd(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
        }

        /// <summary>
        /// Update all objects
        /// </summary>
        /// <param name="dt"></param>
        public void Update(float dt)
        {
            var objectsToRemove = new List<GameObject>();

            // Defer creation of new objects
            foreach (GameObject obj in ObjectsToCreate)
            {
                Objects.Add(obj);
                LayerManager.Objects.Add(obj);
            }
            ObjectsToCreate.Clear();

            // Iterate over each object and update them
            foreach (var obj in Objects)
            {
                // Call the update function
                // If we are in editor, we only want to draw our layers
                if (!GameManager.Paused)
                {
                    obj.Update(dt);

                    // Defer collisions
                    if (obj.Collideable)
                    {
                        CollisionHandler.Collisions.Add(obj);
                    }
                }

                // Defer destruction
                if (obj.IsDestroyed)
                {
                    objectsToRemove.Add(obj);
                }
            }

            // Destroy all objects marked for destroy
            foreach (var obj in objectsToRemove)
            {
                Objects.Remove(obj);
            }

            objectsToRemove.Clear();
        }

        /// <summary>
        /// Gets the name of a layer
        /// </summary>
        /// <param name="selectedLayer"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string GetName(object selectedLayer)
        {
            if (selectedLayer is Layer)
            {
                return ((Layer)selectedLayer).Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the selected tile index
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static int GetSelectedTileID()
        {
            if (Inspector.SelectedLayer is Layer)
            {
                Layer layer = (Layer)Inspector.SelectedLayer;

                if (layer.Type == LayerType.Tile)
                {
                    return ((Layer)(Inspector.SelectedLayer)).Tilemaps[0].SelectedTile;
                }
            }

            return -1;
        }
    }
}
