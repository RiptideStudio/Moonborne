
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
using Moonborne.Game.Objects;
using System.Linq;
using System.Runtime.InteropServices;
using Moonborne.Engine;

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
        public static float ZoomScale = 0.25f; // How much we zoom when scrolling mouse
        public static string NewLayerName = "Layer";
        public static Layer selectedLayer = null;
        public static string selectedObject;
        public static bool Dragging = false;
        public static bool HoveringOverGameWorld = false;
        public static bool InEditor = true; // Flag for if we're in editor mode or not
        public static float PanSpeed = 8f;
        public static int BrushSize = 1;

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
                SelectedTilemap = new Tilemap("TilesetTest", new int[100, 100], 16, "Tile");
            }
        }

        /// <summary>
        /// Draw tiles in the world
        /// </summary>
        public static void DrawGrid(SpriteBatch spriteBatch)
        {
            // Draw the world grid
            if (SelectedTilemap == null)
                return;

            SelectedTilemap.DrawGrid();

            // Draw the selected tile preview
            if (CanPlaceTile)
            {
                Vector2 worldMouse = InputManager.MouseWorldCoords();
                int tileSize = SelectedTilemap.tileSize;
                Color PreviewColor = new Color(128, 255, 128, 200);

                // Calculate the center tile grid position
                int centerX = (int)worldMouse.X / tileSize;
                int centerY = (int)worldMouse.Y / tileSize;

                // Determine the brush size range
                int halfBrush = BrushSize / 2;

                // Loop through the brush size area
                for (int x = centerX - halfBrush; x <= centerX + halfBrush; x++)
                {
                    for (int y = centerY - halfBrush; y <= centerY + halfBrush; y++)
                    {
                        // Calculate the screen position of the tile
                        int drawX = x * tileSize;
                        int drawY = y * tileSize;

                        // Define the rectangle for the tile
                        Rectangle rect = new Rectangle(drawX, drawY, tileSize, tileSize);

                        // Draw the preview tile
                        spriteBatch.Draw(SelectedTilemap.tileset, rect, SelectedTilemap.SelectedSourceTileRectangle, PreviewColor);
                    }
                }
            }


            CanPlaceTile = true;
        }



        /// <summary>
        /// Draw tileset editor
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawEditor(SpriteBatch spriteBatch)
        {
            // If we are in editor mode, no logic is executed
            if (InputManager.KeyTriggered(Keys.Q))
            {
                if (InEditor)
                {
                    GameManager.Start();
                }
                else
                {
                    GameManager.Stop();
                }
            }

            if (!InEditor)
                return;

            // Render the ImGui buttons for toggling different
            ImGui.Begin("Tile Editor");

            if (selectedLayer != null)
            {
                ImGui.Text($"Active Layer: {selectedLayer.Name} ({selectedLayer.Type.ToString()})");
            }

            // Settings
            if (ImGui.CollapsingHeader("Settings"))
            {
                if (ImGui.Button("Save"))
                {
                    CurrentRoom.Save(CurrentRoom.Name);
                }                
                if (ImGui.Button("Load"))
                {
                    CurrentRoom.Load(CurrentRoom.Name);
                }
            }

            // Brush settings
            if (ImGui.CollapsingHeader("Brush Settings"))
            {
                ImGui.SliderInt("Brush Size", ref BrushSize, 1, 10);
            }

            // Properties of tile editor
            if (ImGui.CollapsingHeader("Properties"))
            {
                ImGui.Checkbox("Show Preview", ref ShowPreview);
                ImGui.Checkbox("Show Grid", ref DebugDraw);
                ImGui.Checkbox("Can Edit", ref CanEdit);
                ImGui.Checkbox("Debug Mode", ref GameManager.DebugMode);
            }

            // Do not allow us to place tiles if we are hovering over or interacting w/editor
            if (ImGui.IsWindowHovered() || ImGui.IsAnyItemHovered())
            {
                CanPlaceTile = false;
                HoveringOverGameWorld = false;
            }

            // Display a list of all SelectedTilemap layers and let us select the one we are working on
            if (ImGui.CollapsingHeader("Layers"))
            {
                foreach (var layer in LayerManager.Layers)
                {
                    // Can't edit locked layers
                    if (layer.Value.Locked)
                        continue;

                    Vector2 buttonSize = new Vector2(160, 24);

                    if (ImGui.Selectable(layer.Value.Name, selectedLayer == layer.Value))
                    {
                        // Select different tile layers
                        if (layer.Value.Type == LayerType.Tile)
                        {
                            SelectedTilemap = layer.Value.Tilemaps[0];
                        }
                        else if (layer.Value.Type == LayerType.Object)
                        {
                            SelectedTilemap = null;
                        }
                        selectedLayer = layer.Value;
                    }
                }

                // Add a new layer
                Vector2 newTilemapButton = new Vector2(160, 48);
            }

            // Display the properties of each layer
            if (selectedLayer != null)
            {
                if (ImGui.CollapsingHeader("Layer Properties"))
                {
                    selectedLayer.DrawSettings();
                }

                // Let us pan!
                if (selectedLayer.Type != LayerType.Tile)
                {
                    HoveringOverGameWorld = true;

                    if (ImGui.IsWindowHovered() || ImGui.IsAnyItemHovered())
                    {
                        HoveringOverGameWorld = false;
                    }
                }
            }

            if (selectedLayer != null && selectedLayer.Type == LayerType.Object)
            {
                if (ImGui.CollapsingHeader("Manage Objects"))
                {
                    // Display a list of all objects, and allow us to drag them into the game
                    var list = ObjectLibrary.GetAllGameObjectNames();

                    if (ImGui.TreeNodeEx("Create Object"))
                    {
                        foreach (var name in list)
                        {
                            // Select the object we want to drag into the game
                            ImGui.Selectable(name, selectedObject == name);

                            if (ImGui.BeginDragDropSource())
                            {
                                ImGui.Text($"Place: {name}"); // Visual feedback during dragging
                                Dragging = true;
                                selectedObject = name;
                                ImGui.EndDragDropSource();
                            }
                        }
                        ImGui.TreePop();
                    }

                    // Drag object into world
                    if (InputManager.MouseLeftReleased() && Dragging)
                    {
                        Vector2 position = InputManager.MouseWorldCoords();
                        Dragging = false;
                        var newObject = ObjectLibrary.CreateObject(selectedObject, position, selectedLayer.Name);
                    }

                    // Show a list of all objects on this layer
                    if (ImGui.TreeNodeEx("Objects"))
                    {
                        foreach (GameObject obj in selectedLayer.Objects)
                        {
                            ImGui.Selectable(obj.GetType().Name);
                        }
                        ImGui.TreePop();
                    }

                }
            }


            if (ImGui.CollapsingHeader("Create New Layer"))
            {

                if (ImGui.InputText("Layer Name", ref NewLayerName, 20))
                {
                }

                if (ImGui.Button("Add Tile Layer"))
                {
                    string layerName = NewLayerName;
                    Layer layer = new Layer(1, () => Camera.Transform, LayerType.Tile);
                    Tilemap tilemap = new Tilemap("TilesetTest", new int[100, 100], 16, layerName);
                    LayerManager.AddTilemapLayer(layer, tilemap, layerName);
                }

                // Create a new object layer
                if (ImGui.Button("Add Object Layer"))
                {
                    LayerManager.AddLayer(new Layer(1, () => Camera.Transform, LayerType.Object), NewLayerName);
                }
            }

            ImGui.NewLine();
            ImGui.End();

            if (HoveringOverGameWorld)
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

                if (InputManager.KeyDown(Keys.W))
                {
                    Camera.TargetPosition.Y -= PanSpeed;
                }

                if (InputManager.KeyDown(Keys.A))
                {
                    Camera.TargetPosition.X -= PanSpeed;
                }

                if (InputManager.KeyDown(Keys.S))
                {
                    Camera.TargetPosition.Y += PanSpeed;
                }

                if (InputManager.KeyDown(Keys.D))
                {
                    Camera.TargetPosition.X += PanSpeed;
                }
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

            // Update our camera. We want to pan with WASD 
            Camera.TargetZoom = Math.Clamp(Camera.TargetZoom, 0.25f, 4f);

            // Update our tile selector
            if (SelectedTilemap != null)
            {
                UpdateTileSelector(spriteBatch);
            }
        }

        public static void UpdateTileSelector(SpriteBatch spriteBatch)
        {
            // Show the tileset preview for selecting tiles
            if (ShowPreview)
            {
                SelectedTilemap.DrawTilesetPreview(spriteBatch, PreviewX, PreviewY);
            }

            // Actually place tiles
            if (CanEdit)
            {
                SelectedTilemap.HandleTileSelection(spriteBatch, PreviewX, PreviewY);
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

            // Update SelectedTilemap with values
            PreviewZoom = Math.Clamp(PreviewZoom, 0.5f, 2f);
            SelectedTilemap.PreviewZoom = PreviewZoom;
        }
    }
}