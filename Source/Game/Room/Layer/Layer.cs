
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        UI
    }

    public class Layer
    {
        private readonly Func<Matrix> GetTransform;
        public bool Collideable = false;
        public bool Visible = true;
        public int Depth = 0; 
        public bool Locked { get; set; } = false;
        public SpriteSortMode SortMode { get; set; } = SpriteSortMode.Deferred; // The way sprites are sorted
        public BlendState BlendState { get; set; } = BlendState.AlphaBlend; // Type of blend state used
        public SamplerState SamplerState { get; set; } = SamplerState.PointClamp; // Sampler state used (usually pixel)
        public Matrix Transform => GetTransform(); // The matrix used to transform the layer
        public List<GameObject> Objects { get; set; } = new List<GameObject>(); // Objects on this layer
        public List<Tilemap> Tilemaps { get; set; } = new List<Tilemap>(); // Tiles on this layer
        public LayerType Type {  get; set; } = LayerType.Object;
        public string Name { get; set; } = string.Empty;
        
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
            Objects.Add(obj);
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
            // Global layer flags
            ImGui.Checkbox("Visible", ref Visible);
            if (ImGui.InputInt("Depth", ref Depth))
            {
                LayerManager.Sort();
            }

            // Type specific flags
            switch (Type)
            {
                case LayerType.Object:

                    break;


                case LayerType.Tile:
                    ImGui.Checkbox("Collidable", ref Collideable);

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
            // Render each object
            foreach (var obj in Objects)
            {
                obj.Draw(spriteBatch);
            }

            // Render each tilemap
            foreach (var tileMap in Tilemaps)
            {
                tileMap.Draw(spriteBatch);
            }

            // Render static managers
            DrawStaticManagers(spriteBatch);
        }

        /// <summary>
        /// Draw static managers
        /// </summary>
        public void DrawStaticManagers(SpriteBatch spriteBatch)
        {
            if (Name == "Dialogue") DialogueManager.DrawDialogueBox();
            if (Name == "Inventory") InventoryManager.Draw(spriteBatch);
            if (Name == "TileEditorWorld") RoomEditor.DrawGrid(spriteBatch);
            if (Name == "RoomEditor") RoomEditor.DrawEditor(spriteBatch);
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
            var collideableObjects = new List<GameObject>();

            // Iterate over each object and update them
            foreach (var obj in Objects)
            {
                // Call the update function
                obj.Update(dt);

                // Defer collisions
                if (obj.Collideable)
                {
                    CollisionHandler.Collisions.Add(obj);
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
        }
    }
}
