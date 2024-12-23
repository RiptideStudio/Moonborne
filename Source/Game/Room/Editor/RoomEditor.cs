
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moonborne.Graphics;
using Moonborne.Input;
using Microsoft.Xna.Framework.Input;

namespace Moonborne.Game.Room
{
    public static class RoomEditor
    {
        public static bool DebugDraw = true;
        public static int SelectedTile = 0;
        public static int[,] grid = new int[100,100];

        /// <summary>
        /// Draw the tileset and its grid
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tileset"></param>
        /// <param name="grid"></param>
        /// <param name="tileSize"></param>
        /// <param name="tilesetColumns"></param>
        public static void DrawGrid(SpriteBatch spriteBatch, Texture2D tileset, int tileSize, int tilesetColumns)
        {
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
        /// Select tiles to place
        /// </summary>
        /// <param name="tileset"></param>
        /// <param name="tileSize"></param>
        /// <param name="tilesetColumns"></param>
        /// <param name="previewX"></param>
        /// <param name="previewY"></param>
        public static void HandleTileSelection(Texture2D tileset, int tileSize, int tilesetColumns, int previewX, int previewY)
        {
            if (InputManager.KeyTriggered(Keys.T))
            {
                DebugDraw = !DebugDraw;
            }

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
        public static void DrawTilesetPreview(SpriteBatch spriteBatch, Texture2D tileset, int tileSize, int tilesetColumns, int previewX, int previewY)
        {
            int rows = tileset.Height / tileSize;
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
    }
}