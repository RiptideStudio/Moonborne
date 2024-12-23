
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

namespace Moonborne.Game.Room
{
    public class Tilemap
    {
        public bool DebugDraw = true;
        public int SelectedTile = 0;
        public int[,] grid = new int[100, 100];
        public Texture2D tileset;
        public int tileSize = 16;
        public int tilesetColumns = 10;
        public string Name = "Tilemap";

        /// <summary>
        /// New tilemap
        /// </summary>
        /// <param name="grid_"></param>
        public Tilemap(Texture2D tileset_, int[,] grid_,int tileSize_,int columns_)
        {
            tileset = tileset_;
            grid = grid_;
            tileSize = tileSize_;
            tilesetColumns = columns_;
        }

        /// <summary>
        /// Draw the tileset and its grid
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tileset"></param>
        /// <param name="grid"></param>
        /// <param name="tileSize"></param>
        /// <param name="tilesetColumns"></param>
        public void DrawGrid(SpriteBatch spriteBatch)
        {
            if (InputManager.KeyTriggered(Keys.T))
            {
                DebugDraw = !DebugDraw;
            }

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    if (DebugDraw)
                    {
                        Vector2 gridPosX = new Vector2(0, y * tileSize);
                        Vector2 gridPosY = new Vector2(x * tileSize, 0);
                        SpriteManager.DrawRectangle(gridPosX, tileSize * grid.GetLength(0), 1, Color.White);
                        SpriteManager.DrawRectangle(gridPosY, 1, tileSize * grid.GetLength(1), Color.White);
                    }

                    int tileId = grid[x, y];

                    if (tileId <= 0)
                        continue;

                    int tilesetX = (tileId % tilesetColumns) * tileSize;
                    int tilesetY = (tileId / tilesetColumns) * tileSize+1;

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
        /// Select tiles to place
        /// </summary>
        /// <param name="tileset"></param>
        /// <param name="tileSize"></param>
        /// <param name="tilesetColumns"></param>
        /// <param name="previewX"></param>
        /// <param name="previewY"></param>
        public void HandleTileSelection(int previewX, int previewY)
        {
            if (InputManager.MouseLeftPressed())
            {
                int mouseX = (int)InputManager.MouseUIPosition.X;
                int mouseY = (int)InputManager.MouseUIPosition.Y;

                // Calculate which tile was clicked
                int gridX = (mouseX - previewX) / tileSize;
                int gridY = (mouseY - previewY) / tileSize;

                // Check if the mouse is within the preview area
                int rows = tileset.Height / tileSize;
                if (mouseX >= previewX && mouseX < previewX + tilesetColumns * tileSize &&
                    mouseY >= previewY && mouseY < previewY + rows * tileSize)
                {

                    // Determine the selected tile ID
                    SelectedTile = gridY * tilesetColumns + gridX;
                    return;
                }

                Vector2 worldMouse = InputManager.MouseWorldCoords();
                gridX = ((int)worldMouse.X) / tileSize;
                gridY = ((int)worldMouse.Y) / tileSize;

                // Ensure the click is within the grid bounds
                if (gridX >= 0 && gridX < grid.GetLength(0) && gridY >= 0 && gridY < grid.GetLength(1))
                {
                    grid[gridX, gridY] = SelectedTile; // Place the selected tile in the grid
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
                    int drawX = previewX + x * tileSize;
                    int drawY = previewY + y * tileSize;

                    // Source rectangle for the tile in the tileset
                    Rectangle sourceRectangle = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

                    // Draw the tile
                    spriteBatch.Draw(tileset, new Rectangle(drawX, drawY, tileSize, tileSize), sourceRectangle, Color.White);

                    // Highlight the selected tile with an outline
                    if (tileId == SelectedTile)
                    {
                        // Draw outline using pixelTexture
                        SpriteManager.DrawRectangle(drawX, drawY, tileSize, tileSize, Color.Red);
                    }
                }
            }
        }

        /// <summary>
        /// Save a tilemap to JSON
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string name)
        {
            string contentFolderPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Content\Rooms"));
            Directory.CreateDirectory(contentFolderPath); // Ensure the directory exists
            string filePath = Path.Combine(contentFolderPath, name+".json");

            var tiles = new List<Dictionary<string, int>>();

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    int tileId = grid[x, y];

                    if (tileId > 0) // Only save non-empty tiles
                    {
                        tiles.Add(new Dictionary<string, int>
                        {
                            { "x", x },
                            { "y", y },
                            { "tileId", tileId }
                        });
                    }
                }
            }

            var tilemapData = new
            {
                TileSize = tileSize,
                TilesetColumns = tilesetColumns,
                Tiles = tiles
            };

            // Serialize to JSON
            string json = JsonSerializer.Serialize(tilemapData, new JsonSerializerOptions { WriteIndented = true });

            // Write to file
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Load a tileset
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public void Load(string name)
        {
            string contentFolderPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Content\Rooms"));
            Directory.CreateDirectory(contentFolderPath); // Ensure the directory exists
            string filePath = Path.Combine(contentFolderPath, name + ".json");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file {filePath} does not exist.");

            string json = File.ReadAllText(filePath);

            var tilemapData = JsonSerializer.Deserialize<TilemapData>(json);

            // Ensure grid size matches the loaded data
            grid = new int[100, 100]; // Default to your existing size; adjust if dynamic size is needed

            foreach (var tile in tilemapData.Tiles)
            {
                grid[tile.x, tile.y] = tile.tileId;
            }

            tileSize = tilemapData.TileSize;
            tilesetColumns = tilemapData.TilesetColumns;
        }
    }
}