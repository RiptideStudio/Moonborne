
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Moonborne.Graphics;
using Moonborne.Input;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using Moonborne.Game.Room;
using System;
using MonoGame.Extended.Tiled;
using Moonborne.Game.Gameplay;
using Moonborne.Engine.UI;
using ImGuiNET;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;

namespace Moonborne.Game.Room
{
    public class Tilemap
    {
        public int SelectedTile = 0;
        public int[,] grid = new int[100, 100];
        public int tileSize = 16;
        public int Height = 0;
        public int tilesetColumns = 10;
        public float PreviewZoom = 1f;
        public string LayerName;
        public float Depth = 1f;
        public string TilesetTextureName;
        public Rectangle SelectedDestTileRectangle;
        public Rectangle SelectedSourceTileRectangle;
        public Dictionary<int, Tile> TileList = new Dictionary<int, Tile>();
        public Layer Layer;

        [JsonIgnore]
        public Texture2D tileset;

        /// <summary>
        /// New tilemap
        /// </summary>
        /// <param name="grid_"></param>
        public Tilemap(string tileset_, int[,] grid_,int tileSize_,string layerName)
        {
            TilesetTextureName = tileset_;
            tileset = SpriteManager.GetRawTexture(tileset_);
            grid = grid_;
            tileSize = tileSize_;
            LayerName = layerName;
            tilesetColumns = tileset.Width / 16;       
        }

        /// <summary>
        /// Recalculates tileset properties
        /// </summary>
        public void SetTexture(string tex)
        {
            tileset = SpriteManager.GetRawTexture(tex);
            TilesetTextureName = tex;
            tilesetColumns = tileset.Width / 16;
        }

        /// <summary>
        /// Draw the actual tiles in the tilemap
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Normalize depth
            tileset = SpriteManager.GetRawTexture(TilesetTextureName);

            foreach (var tile in TileList)
            {
                if (tile.Value.CellData <= 0)
                    continue;

                int tilesetX = (tile.Value.CellData % tilesetColumns) * tileSize;
                int tilesetY = (tile.Value.CellData / tilesetColumns) * tileSize;

                spriteBatch.Draw(
                     tileset,
                     new Rectangle(tile.Value.x * tileSize, tile.Value.y * tileSize, tileSize, tileSize), // Position and size in the grid
                     new Rectangle(tilesetX, tilesetY, tileSize, tileSize),        // Source rectangle from the tileset
                     Color.White,
                     0,
                     Vector2.Zero,
                     SpriteEffects.None,
                     Depth
                 );
            }
        }

        /// <summary>
        /// Select tiles to place
        /// </summary>
        /// <param name="tileset"></param>
        /// <param name="tileSize"></param>
        /// <param name="tilesetColumns"></param>
        /// <param name="previewX"></param>
        /// <param name="previewY"></param>
        public void HandleTileSelection(SpriteBatch spriteBatch, int previewX, int previewY)
        {
            // Get the mouse position in screen coordinates
            var mousePos = ImGui.GetMousePos();

            // Get the ImGui window position and size
            var windowPos = ImGui.GetWindowPos(); // Top-left corner of the ImGui window

            // Convert mouse position to local space relative to the window and correct for zoom
            float contentOffsetY = ImGui.GetCursorScreenPos().Y - windowPos.Y;
            float contentOffsetX = ImGui.GetCursorScreenPos().X - windowPos.X;

            // Calculate the local mouse position relative to the tileset
            float localMouseX = (mousePos.X - windowPos.X - previewX - contentOffsetX) / PreviewZoom;
            float localMouseY = (mousePos.Y - windowPos.Y - previewY - contentOffsetY) / PreviewZoom;

            // Ensure alignment by snapping to integer values after scaling
            int snappedMouseX = (int)(localMouseX);
            int snappedMouseY = (int)(localMouseY);

            // Calculate grid coordinates based on scaled mouse position
            int gridX = (snappedMouseX / tileSize);
            int gridY = (snappedMouseY / tileSize);

            // Ensure grid coordinates are within bounds
            int rows = tileset.Height / tileSize;
            if (ImGui.IsWindowHovered())
            {
                // Adjust zoom level with mouse wheel
                if (InputManager.MouseWheelDown())
                {
                    RoomEditor.PreviewZoom = Math.Max(0.25f, RoomEditor.PreviewZoom - 0.25f); // Minimum zoom level
                }

                if (InputManager.MouseWheelUp())
                {
                    RoomEditor.PreviewZoom = Math.Min(4.0f, RoomEditor.PreviewZoom + 0.25f); // Maximum zoom level
                }

                // Determine the selected tile ID
                if (InputManager.MouseLeftPressed())
                {
                    SelectedTile = gridY * tilesetColumns + gridX;

                    SelectedSourceTileRectangle = new Rectangle(
                        gridX * tileSize, // Source X in the tileset
                        gridY * tileSize, // Source Y in the tileset
                        tileSize,
                        tileSize
                    );

                    // Adjust world position and size using zoom
                    SelectedDestTileRectangle = new Rectangle(
                        (int)(gridX * tileSize * PreviewZoom + previewX),
                        (int)(gridY * tileSize * PreviewZoom + previewY),
                        (int)(tileSize * PreviewZoom),
                        (int)(tileSize * PreviewZoom)
                    );
                }

                RoomEditor.HoveringOverGameWorld = false;
                RoomEditor.CanPlaceTile = false;
            }
            else
            {
                RoomEditor.HoveringOverGameWorld = true;
            }

            // Paint the tile into the world 
            if (InputManager.MouseLeftDown() && RoomEditor.CanPlaceTile)
            {
                // Get the grid cell we want to place the tile in
                Vector2 worldMouse = InputManager.MouseWorldCoords();
                int centerX = (int)worldMouse.X / tileSize;
                int centerY = (int)worldMouse.Y / tileSize;

                // Iterate over the area defined by the brush size
                int halfBrush = RoomEditor.BrushSize / 2;
                for (int x = centerX - halfBrush; x <= centerX + halfBrush; x++)
                {
                    for (int y = centerY - halfBrush; y <= centerY + halfBrush; y++)
                    {
                        // Ensure the brush stays within grid bounds
                        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
                        {
                            // Compute a unique key for the tile based on x and y
                            int tileKey = x + y * grid.GetLength(0);

                            // Check if the tile exists in the dictionary
                            if (TileList.TryGetValue(tileKey, out var tileToRemove))
                            {
                                TileList.Remove(tileKey); // Remove the tile from the dictionary
                                grid[x, y] = 0;           // Clear the tile from the grid
                            }

                            // Place the new tile
                            grid[x, y] = SelectedTile;

                            // Add the new tile to the dictionary
                            TileList[tileKey] = new Tile(x, y, SelectedTile, RoomEditor.selectedLayer.Height);
                        }
                    }
                }

            }

            // Erase the tile with right-click
            if (InputManager.MouseRightDown())
            {
                // Get the grid cell we want to place the tile in
                Vector2 worldMouse = InputManager.MouseWorldCoords();
                gridX = ((int)worldMouse.X) / tileSize;
                gridY = ((int)worldMouse.Y) / tileSize;

                // Ensure the click is within the grid bounds
                // Iterate over the area defined by the brush size
                int halfBrush = RoomEditor.BrushSize / 2;
                int centerX = (int)worldMouse.X / tileSize;
                int centerY = (int)worldMouse.Y / tileSize;
                for (int x = centerX - halfBrush; x <= centerX + halfBrush; x++)
                {
                    for (int y = centerY - halfBrush; y <= centerY + halfBrush; y++)
                    {
                        // Ensure the brush stays within grid bounds
                        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
                        {
                            // Compute a unique key for the tile based on x and y
                            int tileKey = x + y * grid.GetLength(0);

                            // Check if the tile exists in the dictionary
                            if (TileList.TryGetValue(tileKey, out var tileToRemove))
                            {
                                TileList.Remove(tileKey); // Remove the tile from the dictionary
                                grid[x, y] = 0;           // Clear the tile from the grid
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Draws a tileset preview
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tileset"></param>
        /// <param name="tileSize"></param>
        /// <param name="tilesetColumns"></param>
        /// <param name="previewX"></param>
        /// <param name="previewY"></param>
        public void DrawTilesetPreview(SpriteBatch spriteBatch, int previewX, int previewY)
        {
            int rows = tileset.Height / tileSize; // Assuming same dimensions
            int columns = tilesetColumns;

            IntPtr pixelTex = ImGuiManager.imGuiRenderer.BindTexture(SpriteManager.PixelTexture);
            IntPtr tilesetTex = ImGuiManager.imGuiRenderer.BindTexture(tileset);
            ImGui.Image(tilesetTex, new System.Numerics.Vector2((int)(tileset.Width * PreviewZoom), (int)(tileset.Height * PreviewZoom)));

            // Get the draw list for custom rendering
            var drawList = ImGui.GetWindowDrawList();

            // Get the position of the ImGui image
            var imgPos = ImGui.GetItemRectMin(); // Top-left corner of the image
            var imgSize = ImGui.GetItemRectSize(); // Size of the image

            // Draw tiles and gridlines directly
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    int tileId = y * columns + x;

                    // Calculate position for this tile in the image space
                    float drawX = imgPos.X + x * tileSize * PreviewZoom;
                    float drawY = imgPos.Y + y * tileSize * PreviewZoom;

                    // Calculate the UV coordinates for the tile in the texture
                    float uvMinX = (float)x / columns;
                    float uvMinY = (float)y / rows;
                    float uvMaxX = (float)(x + 1) / columns;
                    float uvMaxY = (float)(y + 1) / rows;

                    // Draw the tile using ImGui draw list
                    drawList.AddImage(
                        tilesetTex, // Texture ID
                        new System.Numerics.Vector2(drawX, drawY), // Top-left corner
                        new System.Numerics.Vector2(drawX + tileSize * PreviewZoom, drawY + tileSize * PreviewZoom), // Bottom-right corner
                        new System.Numerics.Vector2(uvMinX, uvMinY), // UV top-left
                        new System.Numerics.Vector2(uvMaxX, uvMaxY) // UV bottom-right
                    );

                    // Draw gridlines around the tile
                    var gridLineColor = new System.Numerics.Vector4(1.0f, 0.0f, 0.0f, 0.3f); // Red with alpha
                    drawList.AddLine(
                        new System.Numerics.Vector2(drawX, drawY), // Top-left
                        new System.Numerics.Vector2(drawX + tileSize * PreviewZoom, drawY), // Top-right
                        ImGui.ColorConvertFloat4ToU32(gridLineColor)
                    );
                    drawList.AddLine(
                        new System.Numerics.Vector2(drawX, drawY), // Top-left
                        new System.Numerics.Vector2(drawX, drawY + tileSize * PreviewZoom), // Bottom-left
                        ImGui.ColorConvertFloat4ToU32(gridLineColor)
                    );
                    drawList.AddLine(
                        new System.Numerics.Vector2(drawX + tileSize * PreviewZoom, drawY), // Top-right
                        new System.Numerics.Vector2(drawX + tileSize * PreviewZoom, drawY + tileSize * PreviewZoom), // Bottom-right
                        ImGui.ColorConvertFloat4ToU32(gridLineColor)
                    );
                    drawList.AddLine(
                        new System.Numerics.Vector2(drawX, drawY + tileSize * PreviewZoom), // Bottom-left
                        new System.Numerics.Vector2(drawX + tileSize * PreviewZoom, drawY + tileSize * PreviewZoom), // Bottom-right
                        ImGui.ColorConvertFloat4ToU32(gridLineColor)
                    );

                    // Highlight the selected tile with an overlay
                    if (tileId == SelectedTile)
                    {
                        var selectedColor = new System.Numerics.Vector4(0.0f, 1.0f, 0.0f, 0.3f); // Green with alpha
                        drawList.AddRectFilled(
                            new System.Numerics.Vector2(drawX, drawY),
                            new System.Numerics.Vector2(drawX + tileSize * PreviewZoom, drawY + tileSize * PreviewZoom),
                            ImGui.ColorConvertFloat4ToU32(selectedColor)
                        );
                    }
                }
            }
        }

    }
}