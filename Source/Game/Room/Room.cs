using Moonborne.Game.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Moonborne.Game.Room
{
    public class Room
    {
        public (int TilesetIndex, int TileIndex)[,] Tilemap { get; private set; }
        public List<GameObject> Objects = new List<GameObject>();
        public List<Texture2D> Tilesets;

        public int TileSize = 16;
        public int Width = 100;
        public int Height = 100;

        /// <summary>
        /// Create a new room given a list of tilesets
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="tilesets"></param>
        /// <param name="tileSize"></param>
        public Room(int width, int height, List<Texture2D> tilesets, int tileSize)
        {
            Width = width;
            Height = height;
            Tilemap = new (int, int)[height, width];
            Tilesets = tilesets;
            TileSize = tileSize;
        }

        /// <summary>
        /// Draw a room and all of its tiles and objects
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraPosition"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var (tilesetIndex, tileIndex) = Tilemap[y, x];

                    if (tilesetIndex < 0 || tileIndex < 0) // Only draw valid tiles
                    {
                        continue;
                    }

                    Texture2D tileset = Tilesets[tilesetIndex];
                    int tilesetColumns = tileset.Width / TileSize;

                    int sourceX = (tileIndex % tilesetColumns) * TileSize;
                    int sourceY = (tileIndex / tilesetColumns) * TileSize;

                    spriteBatch.Draw(
                        tileset,
                        new Vector2(x * TileSize, y * TileSize),
                        new Rectangle(sourceX, sourceY, TileSize, TileSize),
                        Color.White
                    );
                }
            }
        }

        /// <summary>
        /// Add an object to be rendered
        /// </summary>
        /// <param name="obj"></param>
        public void AddObject(GameObject obj)
        {
            Objects.Add(obj);
        }

        /// <summary>
        /// Set a tile in the tilemap
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tilesetIndex"></param>
        /// <param name="tileIndex"></param>
        public void SetTile(int x, int y, int tilesetIndex, int tileIndex)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                Tilemap[y, x] = (tilesetIndex, tileIndex); // Store the tileset and tile index
            }
        }
    }
}