
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moonborne.Graphics;
using Moonborne.Input;
using Microsoft.Xna.Framework.Input;
using System.IO;
using ImGuiNET;
using System;
using Moonborne.Graphics.Camera;
using Moonborne.Game.Inventory;
using System.Collections.Generic;
using System.Text.Json;
using Moonborne.Graphics.Window;
using MonoGame.Extended.Collisions.Layers;

namespace Moonborne.Game.Room
{
    public static class RoomEditor
    {
        public static Tilemap SelectedTilemap;
        public static Room CurrentRoom;
        public static bool DebugDraw = false;
        public static bool ShowPreview = true;
        public static bool CanEdit = true;
        public static bool CanPlaceTile = true;
        public static int PreviewX = 0;
        public static int PreviewY = 0;
        public static float PreviewZoom = 1f;
        public static float ZoomScale = 0.2f; // How much we zoom when scrolling mouse
        public static string NewLayerName = "Layer";
        public static Layer selectedLayer = null;

        /// <summary>
        /// Create a default SelectedTilemap
        /// </summary>
        public static void Initialize()
        {
            CurrentRoom = new Room();

            if (File.Exists("Content/Rooms/Room.json"))
            {
                CurrentRoom.Load("Room");
            }
            else
            {
                // No room data was found, generate a base tileset to work from
                SelectedTilemap = new Tilemap(SpriteManager.GetTexture("TilesetTest"), new int[100, 100], 16, 10, "Tile");
            }
        }

        /// <summary>
        /// Draw tiles in the world
        /// </summary>
        public static void DrawGrid(SpriteBatch spriteBatch)
        {
            // Draw the world grid
            SelectedTilemap.DrawGrid();

            // Draw the selected tile preview
            if (CanPlaceTile)
            {
                Vector2 worldMouse = InputManager.MouseWorldCoords();
                int tileSize = SelectedTilemap.tileSize;
                Color PreviewColor = new Color(255, 255, 255, 200);
                int drawX = (int)worldMouse.X;
                int drawY = (int)worldMouse.Y;

                Rectangle rect = new Rectangle((drawX / 16) * 16, (drawY / 16) * 16, tileSize, tileSize);

                spriteBatch.Draw(SelectedTilemap.tileset, rect, SelectedTilemap.SelectedSourceTileRectangle, PreviewColor);
            }

            CanPlaceTile = true;
        }



        /// <summary>
        /// Draw tileset editor
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawEditor(SpriteBatch spriteBatch)
        {
            // Render the ImGui buttons for toggling different
            ImGui.Begin("Tile Editor");

            ImGui.Text($"Active Layer: {SelectedTilemap.LayerName}");
            ImGui.Checkbox("Show Preview", ref ShowPreview);
            ImGui.Checkbox("Show Grid", ref DebugDraw);
            ImGui.Checkbox("Can Edit", ref CanEdit);
            
            // Do not allow us to place tiles if we are hovering over or interacting w/editor
            if (ImGui.IsWindowHovered() || ImGui.IsAnyItemHovered())
            {
                CanPlaceTile = false;
            }
            ImGui.NewLine();

            // Display a list of all SelectedTilemap layers and let us select the one we are working on
            bool selected = false;

            if (ImGui.TreeNodeEx("Select Layers"))
            {
                foreach (var layer in LayerManager.Layers)
                {
                    if (layer.Value.Tilemaps.Count > 0 && layer.Value.Type == LayerType.Tile)
                    {
                        Vector2 buttonSize = new Vector2(160, 24);

                        selected = SelectedTilemap == layer.Value.Tilemaps[0];
                        if (ImGui.Selectable(layer.Value.Name,selected))
                        {
                            SelectedTilemap = layer.Value.Tilemaps[0];
                            selectedLayer = layer.Value;
                        }
                    }
                }

                // Add a new layer
                Vector2 newTilemapButton = new Vector2(160, 48);
                ImGui.TreePop();
            }

            if (selectedLayer != null)
            {
                if (ImGui.TreeNodeEx("Properties")) // Start properties tree
                {
                    ImGui.Checkbox("Visible", ref selectedLayer.Visible);
                    ImGui.Checkbox("Collidable", ref selectedLayer.Collideable);
                    if (ImGui.InputInt("Depth", ref selectedLayer.Depth))
                    {
                        LayerManager.Sort();
                    }

                    ImGui.TreePop(); // End properties tree
                }
            }
            ImGui.NewLine();

            // Menu for creating a new layer
            if (ImGui.TreeNodeEx("Create New Layer"))
            {
                // Create a new tile layer
                if (ImGui.InputText("Layer Name", ref NewLayerName, 20))
                {
                }

                if (ImGui.Button("Add Tile Layer"))
                {
                    string layerName = NewLayerName;
                    Layer layer = new Layer(1, () => Camera.Transform, LayerType.Tile);
                    Tilemap tilemap = new Tilemap(SpriteManager.GetTexture("TilesetTest"), new int[100, 100], 16, 10, layerName);
                    LayerManager.AddTilemapLayer(layer,tilemap, layerName);
                }

                // Create a new object layer
                if (ImGui.Button("Add Object Layer"))
                {
                    LayerManager.AddLayer(new Layer(1, () => Camera.Transform, LayerType.Object, true), "TestObjectLayer");
                }

                ImGui.TreePop();
            }

            ImGui.End();

            // Show the tileset preview for selecting tiles
            if (ShowPreview)
            {
                SelectedTilemap.DrawTilesetPreview(spriteBatch, PreviewX, PreviewY);
            }

            // Actually place tiles
            if (CanEdit)
            {
                SelectedTilemap.HandleTileSelection(spriteBatch,PreviewX, PreviewY);
            }

            // Hotkeys
            if (InputManager.KeyDown(Keys.LeftControl))
            {
                // Quick save
                if (InputManager.KeyTriggered(Keys.S))
                {
                    CurrentRoom.Save(CurrentRoom.Name);
                }                
                
                // Quick load
                if (InputManager.KeyTriggered(Keys.L))
                {
                    CurrentRoom.Load(CurrentRoom.Name);
                }
            }

            // Toggle the debug drawing of grid
            if (InputManager.KeyTriggered(Keys.G))
            {
                DebugDraw = !DebugDraw;
            }
            
            // Disable editor
            if (InputManager.KeyTriggered(Keys.T))
            {
                ShowPreview = !ShowPreview;
                CanEdit = ShowPreview;
            }

            if (!CanPlaceTile)
            {
                // Zoom in and out on the tileset preview
                if (InputManager.MouseWheelDown())
                {
                    PreviewZoom -= 0.25f;
                }

                if (InputManager.MouseWheelUp())
                {
                    PreviewZoom += 0.25f;
                }
            }
            else
            {
                // Zoom in and out from the world
                if (InputManager.MouseWheelDown())
                {
                    Camera.TargetZoom -= ZoomScale;
                }

                if (InputManager.MouseWheelUp())
                {
                    Camera.TargetZoom += ZoomScale;
                }
            }

            // Update SelectedTilemap with values
            Camera.TargetZoom = Math.Clamp(Camera.TargetZoom, 0.25f, 4f);
            PreviewZoom = Math.Clamp(PreviewZoom, 0.5f, 2f);
            SelectedTilemap.PreviewZoom = PreviewZoom;
        }
    }
}