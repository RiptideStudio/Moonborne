
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
using MonoGame.Extended.Screens;
using Moonborne.Engine.UI;
using Moonborne.Engine.Collision;
using System.Runtime.CompilerServices;

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
        public static float ZoomScale = 0.5f; // How much we zoom when scrolling mouse
        public static string NewLayerName = "Layer";
        public static Layer selectedLayer = null;
        public static GameObject selectedObject;
        public static bool Dragging = false;
        public static bool HoveringOverGameWorld = false;
        public static bool InEditor = true; // Flag for if we're in editor mode or not
        public static float PanSpeed = 8f;
        public static int BrushSize = 1;
        public static int WorldSize = 100;
        public static int CellSize = 16;
        public static bool CanDrag = false;

        /// <summary>
        /// Create a default SelectedTilemap
        /// </summary>
        public static void Initialize()
        {

        }

        /// <summary>
        /// Draw tiles in the world
        /// </summary>
        public static void DrawGrid(SpriteBatch spriteBatch)
        {
            // Disable in play mode
            if (!GameManager.Paused)
                return;

            // Draw the world grid
            if (InputManager.KeyDown(Keys.LeftControl))
            {
                if (InputManager.KeyTriggered(Keys.G))
                {
                    DebugDraw = !DebugDraw;
                }
                if (InputManager.KeyTriggered(Keys.Z))
                {
                    RoomManager.Undo();
                }
                if (InputManager.KeyTriggered(Keys.Y))
                {
                    RoomManager.Redo();
                }
            }

            if (DebugDraw)
            {
                SpriteManager.SetDrawAlpha(0.33f);
                int iterations = WorldSize * (16 / CellSize);

                for (int y = 0; y < iterations; y++)
                {
                    Vector2 gridPosX = new Vector2(0, y * CellSize);
                    Vector2 gridPosY = new Vector2(y * CellSize, 0);
                    SpriteManager.DrawRectangle(gridPosX, 16 * WorldSize, 1, Color.White);
                    SpriteManager.DrawRectangle(gridPosY, 1, 16 * WorldSize, Color.White);
                }

                SpriteManager.ResetDraw();
            }

            if (SelectedTilemap == null)
                return;

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
            HoveringOverGameWorld = true;

            // Do not allow us to place tiles if we are hovering over or interacting w/editor
            bool isHoveringAnyWindow = ImGui.GetIO().WantCaptureMouse;
            if (isHoveringAnyWindow)
            {
                CanPlaceTile = false;
                HoveringOverGameWorld = false;
                CanDrag = false;
            }

            if (InputManager.KeyDown(Keys.LeftControl))
            {
                // If we are in editor mode, no logic is executed
                if (InputManager.KeyTriggered(Keys.Q))
                {
                    GameManager.ToggleGamemode();
                }

                // Toggle debug mode
                if (InputManager.KeyTriggered(Keys.B))
                {
                    GameManager.DebugMode = !GameManager.DebugMode;
                    Console.WriteLine($"Toggled Debug Mode: {GameManager.DebugMode}");
                }
            }

            Toolbar.Draw();

            if (!InEditor)
                return;

            Inspector.Draw("Inspector", Inspector.SelectedObject);

            if (selectedLayer == null || selectedLayer.Type == LayerType.Object)
            {
                // Click on objects
                if (InputManager.MouseLeftPressed() && HoveringOverGameWorld)
                {
                    GameObject clickedObject = null;

                    // Iterate over all objects in the scene
                    foreach (var obj in LayerManager.Objects) // Assume SceneManager.Objects holds all active objects
                    {
                        // Check if the mouse position intersects the object's bounding box
                        Vector2 worldCoords = InputManager.MouseWorldCoords();
                        Rectangle rect = new Rectangle((int)worldCoords.X, (int)worldCoords.Y, 8, 8);

                        if (CollisionHandler.IsColliding(rect, obj.Hitbox))
                        {
                            clickedObject = obj;
                            CanDrag = true;
                            break; // Stop checking once we find the first object
                        }
                    }

                    // If an object was clicked, set it as the selected object
                    if (clickedObject != null && !clickedObject.Layer.Locked)
                    {
                        SelectLayer(clickedObject.Layer);
                        Console.WriteLine($"Selected {clickedObject.GetType().Name} on {selectedLayer.Name}");
                    }
                    Inspector.SelectedObject = clickedObject;
                }
            }

            SceneEditor.Draw();
            ConsoleEditor.Draw();
            ObjectEditor.Draw();
            LevelSelectEditor.Draw();

            // Draw prefab editor if we are in it
            if (PrefabEditor.IsActive)
            {
                PrefabEditor.Draw(spriteBatch);
            }

            // Render the ImGui buttons for toggling different
            ImGui.Begin("World Editor");

            // Grid settings
            if (ImGui.Button("Reload Assets"))
            {
                SpriteManager.ReloadTextures();
            }
            ImGui.SliderInt("Cell Step Size", ref CellSize, 1, 16);
            ImGui.SliderInt("World Size", ref WorldSize, 10, 100);

            if (SelectedTilemap != null)
            {
                // Tile editing
                ImGui.SliderInt("Brush Size", ref BrushSize, 1, 10);
                ImGui.Checkbox("Show Preview", ref ShowPreview);
                ImGui.Checkbox("Can Edit", ref CanEdit);
                ImGui.Checkbox("Show Grid", ref DebugDraw);

                if (ImGui.TreeNodeEx("Texture"))
                {
                    foreach (var tex in SpriteManager.textures)
                    {
                        if (ImGui.Button(tex.Key))
                        {
                            SelectedTilemap.SetTexture(tex.Key);
                        }
                    }

                    ImGui.TreePop();
                }
            }
            else
            {
                ImGui.Checkbox("Show Grid", ref DebugDraw);
            }
            
            ImGui.End(); // End world editor

            if (HoveringOverGameWorld)
            {
                // Zoom in and out from the world
                if (InputManager.MouseWheelDown())
                {
                    Camera.CameraSize += ZoomScale;
                }

                if (InputManager.MouseWheelUp())
                {
                    // Zoom in towards where our mouse is
                    Camera.CameraSize -= ZoomScale;
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

                // Pan the camera using right click
                if (InputManager.MouseRightDown())
                {
                    InputManager.SetMouseLocked(true);
                    Camera.TargetPosition += InputManager.GetMouseDeltaPosition() / 2;
                }
            }

            // Stop panning
            if (InputManager.MouseRightReleased())
            {
                InputManager.SetMouseLocked(false);
            }

            // Hotkeys
            if (InputManager.KeyDown(Keys.LeftControl))
            {
                // Quick save
                if (InputManager.KeyTriggered(Keys.S))
                {
                    GameManager.Save();
                }

                // Quick load
                if (InputManager.KeyTriggered(Keys.L))
                {
                    CurrentRoom.Load(CurrentRoom.Name);
                }
            }

            // Control the grid cell snap size
            if (InputManager.KeyTriggered(Keys.OemCloseBrackets))
            {
                CellSize += 4;
                CellSize = Math.Clamp(CellSize, 4, 16);
            }
            if (InputManager.KeyTriggered(Keys.OemOpenBrackets))
            {
                CellSize -= 4;
                CellSize = Math.Clamp(CellSize, 4, 16);
            }

            // Update our camera. We want to pan with WASD 
            Camera.CameraSize = Math.Clamp(Camera.CameraSize, 0.25f, Camera.MaxZoom);

            // Update our tile selector
            if (SelectedTilemap != null)
            {
                ImGui.Begin("Tile Editor", ImGuiWindowFlags.NoScrollbar);
                UpdateTileSelector(spriteBatch);
                ImGui.End();
            }
        }

        public static void UpdateTileSelector(SpriteBatch spriteBatch)
        {
            // Actually place tiles
            if (CanEdit)
            {
                SelectedTilemap.HandleTileSelection(spriteBatch, PreviewX, PreviewY);
            }

            // Show the tileset preview for selecting tiles
            if (ShowPreview)
            {
                SelectedTilemap.DrawTilesetPreview(spriteBatch, PreviewX, PreviewY);
            }

            // Toggle the debug drawing of grid
            if (InputManager.KeyDown(Keys.LeftControl))
            {
                // Disable editor
                if (InputManager.KeyTriggered(Keys.T))
                {
                    ShowPreview = !ShowPreview;
                    CanEdit = ShowPreview;
                }
            }

            // Update SelectedTilemap with values
            PreviewZoom = Math.Clamp(PreviewZoom, 0.25f, 4f);
            SelectedTilemap.PreviewZoom = PreviewZoom;
        }

        /// <summary>
        /// Select a new layer
        /// </summary>
        /// <param name="newLayer"></param>
        public static void SelectLayer(Layer newLayer)
        {
            // Reset layer selection if null
            if (newLayer == null)
            {
                selectedLayer = null;
                Inspector.SelectedLayer = null;
                SelectedTilemap = null;
                Console.WriteLine("Selected NULL Layer");
                return;
            }

            // Set selected layers
            selectedLayer = newLayer;
            Inspector.SelectedLayer = selectedLayer;

            // Select different tile layers
            if (newLayer.Type == LayerType.Tile)
            {
                SelectedTilemap = newLayer.Tilemaps[0];
            }
            else if (newLayer.Type == LayerType.Object)
            {
                SelectedTilemap = null;
            }
        }
    }
}