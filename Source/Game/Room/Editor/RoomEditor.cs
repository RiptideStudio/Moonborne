
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
            if (ImGui.TreeNodeEx("Select Layers"))
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
                            
                        }
                        selectedLayer = layer.Value;
                    }
                }

                // Add a new layer
                Vector2 newTilemapButton = new Vector2(160, 48);
                ImGui.TreePop();
            }

            // Display the properties of each layer
            if (selectedLayer != null)
            {
                if (ImGui.TreeNodeEx("Properties")) // Start properties tree
                {
                    selectedLayer.DrawSettings();

                    ImGui.TreePop(); // End properties tree
                }
            }
            ImGui.NewLine();

            ImGui.End();

            // Give us a dropdown for creating objects
            if (selectedLayer != null)
            {
                if (selectedLayer.Type == LayerType.Object)
                {
                    ImGui.Begin("Game Objects");
                    if (ImGui.IsWindowHovered() || ImGui.IsAnyItemHovered())
                    {
                        CanPlaceTile = false;
                    }
                    // Display a list of all objects, and allow us to drag them into the game
                    var list = ObjectLibrary.GetAllGameObjectNames();

                    if (ImGui.TreeNodeEx("Create Object"))
                    {
                        foreach (var name in list)
                        {
                            // Select the object we want to drag into the game
                            if (ImGui.Button(name))
                            {
                                Vector2 position = InputManager.MouseWorldCoords();
                                var newObject = ObjectLibrary.CreateObject(name, position, selectedLayer.Name);
                                Console.WriteLine($"Created {name} at {newObject.Position}");
                            }
                        }
                        ImGui.TreePop();
                    }

                    if (ImGui.TreeNodeEx("Objects"))
                    {
                        foreach (GameObject obj in selectedLayer.Objects)
                        {
                            ImGui.Selectable(obj.GetType().Name);
                        }
                        ImGui.TreePop();
                    }

                    ImGui.End();
                }
            }

            // Create a new layer
            ImGui.Begin("Create Layer");
            if (ImGui.IsWindowHovered() || ImGui.IsAnyItemHovered())
            {
                CanPlaceTile = false;
            }
            if (ImGui.InputText("Layer Name", ref NewLayerName, 20))
            {
            }

            if (ImGui.Button("Add Tile Layer"))
            {
                string layerName = NewLayerName;
                Layer layer = new Layer(1, () => Camera.Transform, LayerType.Tile);
                Tilemap tilemap = new Tilemap(SpriteManager.GetTexture("TilesetTest"), new int[100, 100], 16, 10, layerName);
                LayerManager.AddTilemapLayer(layer, tilemap, layerName);
            }

            // Create a new object layer
            if (ImGui.Button("Add Object Layer"))
            {
                LayerManager.AddLayer(new Layer(1, () => Camera.Transform, LayerType.Object), "TestObjectLayer");
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

            if (CanPlaceTile)
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