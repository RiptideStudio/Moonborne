using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Moonborne.Game.Objects;
using Moonborne.Graphics;
using Moonborne.Input;
using System;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Input;

namespace Moonborne.Game.Room
{
    public static class LevelEditor
    {
        private static List<Texture2D> Tilesets;   // List of all tilesets
        private static int selectedTileset = 0;    // Current tileset index
        private static int selectedTileIndex = 0;  // Selected tile index within the tileset

        /// <summary>
        /// Load the tilesets into the editor.
        /// </summary>
        /// <param name="tilesetList">List of tileset textures</param>
        public static void LoadTilesets()
        {
            // Load your tileset textures
            List<Texture2D> tilesets = new List<Texture2D>
            {
                SpriteManager.GetTexture("JungleTileset")
            };

            RoomManager.CurrentRoom = new Room(100,100,tilesets, 16);
            Tilesets = tilesets;
        }

        /// <summary>
        /// Draw our selector
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawTilesetSelector(SpriteBatch spriteBatch)
        {
            int startX = 10; // Starting position for tileset preview
            int startY = 10;
            int tileSize = 16; // Tile size in pixels
            int tilesetPreviewSize = 400; // Size of the tileset preview box (scaled display)

            Texture2D currentTileset = Tilesets[selectedTileset]; // Currently selected tileset
            int tilesetColumns = currentTileset.Width / tileSize;
            int tilesetRows = currentTileset.Height / tileSize;

            Rectangle previewRect = new Rectangle(startX, startY, tilesetPreviewSize, tilesetPreviewSize);

            // Draw the tileset preview
            spriteBatch.Draw(currentTileset, previewRect, Color.White);

            // Calculate scaling factor for the preview
            float scale = tilesetPreviewSize / (float)(tileSize * tilesetColumns);

            // Draw grid and handle tile selection
            for (int y = 0; y < tilesetRows; y++)
            {
                for (int x = 0; x < tilesetColumns; x++)
                {
                    Rectangle tileRect = new Rectangle(
                        startX + (int)(x * tileSize * scale),
                        startY + (int)(y * tileSize * scale),
                        (int)(tileSize * scale),
                        (int)(tileSize * scale)
                    );

                    Rectangle sourceRect = new Rectangle(
                        x * tileSize, // X position of the tile in the tileset
                        y * tileSize, // Y position of the tile in the tileset
                        tileSize,     // Tile width
                        tileSize      // Tile height
                    );

                    // Highlight the selected tile
                    int tileIndex = y * tilesetColumns + x;
                    if (tileIndex == selectedTileIndex)
                    {
                        spriteBatch.Draw(currentTileset, tileRect, sourceRect, Color.Red);
                    }

                    // Detect mouse clicks
                    if (tileRect.Contains(InputManager.MousePosition) && InputManager.MouseLeftPressed())
                    {
                        selectedTileIndex = tileIndex; // Update selected tile index
                        return;
                    }
                }
            }

            // Place tiles
            if (RoomManager.CurrentRoom != null)
            {
                HandleTilePlacement();
            }

            // Hotkeys for saving and loading
            UpdateHotkeys();
        }

        /// <summary>
        /// Hotkeys for using level editor
        /// </summary>
        public static void UpdateHotkeys()
        {
            if (InputManager.KeyDown(Keys.LeftControl))
            {
                // Quick save
                if (InputManager.KeyTriggered(Keys.S))
                {
                    SaveRoom(RoomManager.CurrentRoom, "Room1");
                }
            }
        }

        /// <summary>
        /// Deals with clicking to place tiles
        /// </summary>
        /// <param name="mouseState"></param>
        /// <param name="currentRoom"></param>
        /// <param name="cameraPosition"></param>
        public static void HandleTilePlacement()
        {
            // Convert mouse position to world position (camera offset)
            Vector2 worldPosition = InputManager.MouseWorldCoords();

            // Convert to tile coordinates
            int tileX = (int)Math.Floor(worldPosition.X / 16); // Assuming 16x16 tiles
            int tileY = (int)Math.Floor(worldPosition.Y / 16);

            // Place the tile
            if (InputManager.MouseLeftPressed())
            {
                RoomManager.CurrentRoom.SetTile(tileX, tileY, selectedTileset, selectedTileIndex);
            }
        }

        /// <summary>
        /// Save a room to JSON
        /// </summary>
        /// <param name="room"></param>
        /// <param name="filePath"></param>
        public static void SaveRoom(Room room, string fileName)
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
            string roomsDirectory = Path.Combine(projectRoot, "Content", "Data", "Rooms");
            string filePath = Path.Combine(roomsDirectory, fileName+".json");

            var roomData = new
            {
                Width = room.Width,
                Height = room.Height,
                Tilemap = room.Tilemap,
                Objects = room.Objects
            };

            string json = JsonConvert.SerializeObject(roomData, Formatting.Indented);
            File.WriteAllText(filePath, json);

            Console.WriteLine($"Room saved to {filePath}"); ;
        }

        /// <summary>
        /// Load a room from JSON
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="tilesets"></param>
        /// <param name="tileSize"></param>
        /// <returns></returns>
        public static Room LoadRoom(string filePath, List<Texture2D> tilesets, int tileSize)
        {
            string json = File.ReadAllText(filePath);
            dynamic roomData = JsonConvert.DeserializeObject(json);

            int width = roomData.Width;
            int height = roomData.Height;

            Room room = new Room(width, height, tilesets, tileSize);

            var tilemapData = roomData.Tilemap.ToObject<(int, int)[,]>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    room.SetTile(x, y, tilemapData[y, x].Item1, tilemapData[y, x].Item2);
                }
            }

            foreach (var obj in roomData.Objects)
            {
                Vector2 position = new Vector2((float)obj.Position.X, (float)obj.Position.Y);
                string type = (string)obj.Type;
                room.AddObject(new Actor());
            }

            return room;
        }
    }
}