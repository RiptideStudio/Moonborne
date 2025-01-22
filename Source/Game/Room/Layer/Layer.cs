
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Engine;
using Moonborne.Engine.Collision;
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
        Transition,
        UI
    }

    public class Layer
    {
        private readonly Func<Matrix> GetTransform;
        public bool Locked = false;
        public bool Collideable { get; set; } = false;
        public bool Visible { get; set; } = true;
        public int Depth { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public SpriteSortMode SortMode = SpriteSortMode.Deferred; // The way sprites are sorted
        public BlendState BlendState = BlendState.NonPremultiplied; // Type of blend state used
        public SamplerState SamplerState = SamplerState.PointClamp; // Sampler state used (usually pixel)
        public Matrix Transform => GetTransform(); // The matrix used to transform the layer
        public List<GameObject> Objects { get; set; } = new List<GameObject>(); // Objects on this layer
        public List<GameObject> ObjectsToCreate { get; set; } = new List<GameObject>(); // Objects on this layer
        public List<Tilemap> Tilemaps { get; set; } = new List<Tilemap>(); // Tiles on this layer
        public LayerType Type {  get; set; } = LayerType.Object;
        public int Height { get; set; } = 1; // Defines the heightmap of this layer
        
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
                // Calculate the depth based on position (y sorting)
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
    }
}
