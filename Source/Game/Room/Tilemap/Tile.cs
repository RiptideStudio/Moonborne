using Moonborne.Game.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonborne.Game.Room
{
    /// <summary>
    /// Define a type of tile (stair up, stair down)
    /// </summary>
    public enum TileType
    {
        None = 0,
        StairUp = 1,
        StairDown = 2
    }


    // Define classes to match JSON structure
    public class TilemapData
    {
        public int TileSize { get; set; }
        public string TilesetName { get; set; }
        public int Height { get; set; }
        public string LayerName {  get; set; }
        public int Depth { get; set; }
        public bool Collideable { get; set; }
        public bool Visible { get; set; }
        public bool IsTransitionLayer { get; set; }
        public List<Dictionary<string, int>> Tiles { get; set; }
    }

    public class TileData
    {
        public int x { get; set; }
        public int y { get; set; }
        public int tileId { get; set; }
        public int Height { get; set; } = 1;
    }

    public class Tile
    {
        public Tile(int x_, int y_, int tileId, int height = 1)
        {
            CellData = tileId;

            // Special tile types
            switch (tileId)
            {
                case (int)TileType.StairDown:
                    TileType = TileType.StairDown;
                    break;                
                
                case (int)TileType.StairUp:
                    TileType = TileType.StairUp;
                    break;
            }
            x = x_;
            y = y_;
            Height = height;
        }

        public int x;
        public int y;
        public int Height = 1;
        public TileType TileType = TileType.None;
        public int CellData = 0; // Index of the tile from the tileset
    }

}
