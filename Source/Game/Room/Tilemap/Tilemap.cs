
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

namespace Moonborne.Game.Room
{
    public class Tilemap
    {
        public int SelectedTile = 0;
        public int[,] grid = new int[100, 100];
        public Texture2D tileset;
        public int tileSize = 16;
        public int tilesetColumns = 10;
        public float PreviewZoom = 1f;
        public string LayerName;
        public string TilesetTextureName;
        public Rectangle SelectedDestTileRectangle;
        public Rectangle SelectedSourceTileRectangle;

        /// <summary>
        /// New tilemap
        /// </summary>
        /// <param name="grid_"></param>
        public Tilemap(string tileset_, int[,] grid_,int tileSize_,string layerName)
        {
            TilesetTextureName = tileset_;
            tileset = SpriteManager.GetTexture(tileset_);
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
            tileset = SpriteManager.GetTexture(tex);
            TilesetTextureName = tex;
            tilesetColumns = tileset.Width / 16;
        }

        /// <summary>
        /// Draw the actual tiles in the tilemap
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    int tileId = grid[x, y];

                    if (tileId <= 0)
                        continue;

                    int tilesetX = (tileId % tilesetColumns) * tileSize;
                    int tilesetY = (tileId / tilesetColumns) * tileSize;

                    spriteBatch.Draw(
                        tileset,
                        new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), // Position and size in the grid
                        new Rectangle(tilesetX, tilesetY, tileSize, tileSize),        // Source rectangle from the tileset
                        Color.White
                    );
                }
            }
        }

        /// <summary>
        /// Draw the grid overlay for tilemaps
        /// </summary>
        public void DrawGrid()
        {
            if (!RoomEditor.DebugDraw)
                return;

            SpriteManager.SetDrawAlpha(0.33f);

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Vector2 gridPosX = new Vector2(0, y * tileSize);
                Vector2 gridPosY = new Vector2(y * tileSize, 0);
                SpriteManager.DrawRectangle(gridPosX, tileSize * grid.GetLength(0), 1, Color.White);
                SpriteManager.DrawRectangle(gridPosY, 1, tileSize * grid.GetLength(1), Color.White);
            }

            SpriteManager.ResetDraw();
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
            int mouseX = (int)(InputManager.MouseUIPosition.X / PreviewZoom);
            int mouseY = (int)(InputManager.MouseUIPosition.Y / PreviewZoom);

            // Calculate which tile was clicked
            int gridX = (mouseX - previewX) / tileSize;
            int gridY = (mouseY - previewY) / tileSize;

            // Check if the mouse is within the preview area
            int rows = tileset.Height / tileSize;

            if (mouseX >= previewX && mouseX < previewX + tilesetColumns * tileSize &&
                mouseY >= previewY && mouseY < previewY + rows * tileSize)
            {
                // Zoom in and out on the tileset preview
                if (InputManager.MouseWheelDown())
                {
                    RoomEditor.PreviewZoom -= 0.25f;
                }

                if (InputManager.MouseWheelUp())
                {
                    RoomEditor.PreviewZoom += 0.25f;
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

                    Vector2 worldPosition = new Vector2(gridX * tileSize, gridY * tileSize);
                    SelectedDestTileRectangle = new Rectangle(
                        (int)worldPosition.X,
                        (int)worldPosition.Y,
                        tileSize,
                        tileSize
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
                            grid[x, y] = SelectedTile; // Place the selected tile in the grid
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
                if (gridX >= 0 && gridX < grid.GetLength(0) && gridY >= 0 && gridY < grid.GetLength(1))
                {
                    grid[gridX, gridY] = 0; // Place the selected tile in the grid
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

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    int tileId = y * columns + x;

                    // Calculate position for this tile in the preview area
                    int drawX = previewX + (int)(x * tileSize * PreviewZoom);
                    int drawY = previewY + (int)(y * tileSize * PreviewZoom);

                    // Source rectangle for the tile in the tileset
                    Rectangle sourceRectangle = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

                    // Draw the tile
                    spriteBatch.Draw(tileset, new Rectangle(drawX, drawY, (int)(tileSize * PreviewZoom), (int)(tileSize * PreviewZoom)), sourceRectangle, Color.White);

                    Color gridLineColor = new Color(255, 0, 0, 75);
                    Color selectedColor = new Color(0, 255, 0, 75);

                    spriteBatch.Draw(
                        SpriteManager.PixelTexture,
                        new Rectangle(drawX, drawY, (int)(tileSize * PreviewZoom), 1), // Top border
                        gridLineColor
                    );

                    spriteBatch.Draw(
                        SpriteManager.PixelTexture,
                        new Rectangle(drawX, drawY, 1, (int)(tileSize * PreviewZoom)), // Left border
                        gridLineColor
                    );

                    spriteBatch.Draw(
                        SpriteManager.PixelTexture,
                        new Rectangle(drawX, drawY + (int)(tileSize * PreviewZoom) - 1, (int)(tileSize * PreviewZoom), 1), // Bottom border
                        gridLineColor
                    );

                    spriteBatch.Draw(
                        SpriteManager.PixelTexture,
                        new Rectangle(drawX + (int)(tileSize * PreviewZoom) - 1, drawY, 1, (int)(tileSize * PreviewZoom)), // Right border
                        gridLineColor
                    );

                    // Highlight the selected tile with an outline
                    if (tileId == SelectedTile)
                    {
                        // Draw outline using pixelTexture
                        Rectangle OverlayRectangle = new Rectangle(drawX, drawY, (int)(tileSize * PreviewZoom), (int)(tileSize * PreviewZoom));
                        SpriteManager.DrawRectangle(OverlayRectangle, selectedColor);
                    }
                }
            }
        }

    }
}