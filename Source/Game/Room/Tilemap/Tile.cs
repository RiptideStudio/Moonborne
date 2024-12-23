using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonborne.Game.Room
{
    // Define classes to match JSON structure
    public class TilemapData
    {
        public int TileSize { get; set; }
        public int TilesetColumns { get; set; }
        public List<TileData> Tiles { get; set; }
    }

    public class TileData
    {
        public int x { get; set; }
        public int y { get; set; }
        public int tileId { get; set; }
    }
}
