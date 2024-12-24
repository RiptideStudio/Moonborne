
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Graphics;
using Moonborne.Graphics.Camera;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Moonborne.Game.Room
{
    public class RoomData
    {
        public string RoomName { get; set; }
        public List<TilemapData> Tilemaps { get; set; }
    }

    public class Room
    {
        public string Name = "Room";

        /// <summary>
        /// Save a room to a json file made of multiple tilemaps
        /// </summary>
        /// <param name="name"></param>
        public void Save(string name)
        {
            string contentFolderPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Content\Rooms"));
            Directory.CreateDirectory(contentFolderPath); // Ensure the directory exists
            string filePath = Path.Combine(contentFolderPath, name + ".json");

            var tilemaps = new List<object>(); // List to store serialized tilemaps

            // Iterate over all layers in the LayerManager
            foreach (var layer in LayerManager.Layers)
            {
                if (layer.Value.Type == LayerType.Tile) // Only process Tile layers
                {
                    foreach (var tilemap in layer.Value.Tilemaps) // Handle multiple tilemaps in the layer
                    {
                        var tiles = new List<Dictionary<string, int>>();

                        // Serialize the grid data for this tilemap
                        for (int y = 0; y < tilemap.grid.GetLength(1); y++)
                        {
                            for (int x = 0; x < tilemap.grid.GetLength(0); x++)
                            {
                                int tileId = tilemap.grid[x, y];

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

                        // Add this tilemap's data to the tilemaps list
                        tilemaps.Add(new
                        {
                            LayerName = tilemap.LayerName,
                            TileSize = tilemap.tileSize,
                            TilesetColumns = tilemap.tilesetColumns,
                            Depth = LayerManager.Layers[tilemap.LayerName].Depth,
                            Collideable = LayerManager.Layers[tilemap.LayerName].Collideable,
                            Visible = LayerManager.Layers[tilemap.LayerName].Visible,
                            Tiles = tiles
                        });
                    }
                }
            }

            // Create a JSON object for the room
            var roomData = new
            {
                RoomName = name,
                Tilemaps = tilemaps
            };

            // Serialize the room data to JSON
            string json = JsonSerializer.Serialize(roomData, new JsonSerializerOptions { WriteIndented = true });

            // Write to file
            File.WriteAllText(filePath, json);
        }


        /// <summary>
        /// Load a room from multiple tilemaps
        /// </summary>
        /// <param name="name"></param>
        public void Load(string name)
        {
            string contentFolderPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Content\Rooms"));
            string filePath = Path.Combine(contentFolderPath, name + ".json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Room file not found: {filePath}");
            }

            // Read the JSON file
            string json = File.ReadAllText(filePath);

            // Deserialize the room data
            var roomData = JsonSerializer.Deserialize<RoomData>(json);

            // Set the room name
            Name = roomData.RoomName;

            // Remove all non static layers
            LayerManager.Clear();

            // Reconstruct tilemaps and add them to their layers
            foreach (var tilemapData in roomData.Tilemaps)
            {
                // Create a new Tilemap instance
                Tilemap tilemap = new Tilemap(
                    SpriteManager.GetTexture("TilesetTest"),
                    new int[100, 100], 
                    tilemapData.TileSize,
                    tilemapData.TilesetColumns,
                    tilemapData.LayerName
                );

                // Reconstruct layers
                Layer layer = new Layer(tilemapData.Depth, () => Camera.Transform, LayerType.Tile);
                layer.Depth = tilemapData.Depth;
                layer.Visible = tilemapData.Visible;
                layer.Collideable = tilemapData.Collideable;
                LayerManager.AddTilemapLayer(layer, tilemap, tilemapData.LayerName);

                // Set our currently selected tilemap
                RoomEditor.SelectedTilemap = tilemap;

                // Populate the grid with tile data
                foreach (var tile in tilemapData.Tiles)
                {
                    tilemap.grid[tile["x"], tile["y"]] = tile["tileId"];
                }
            }
        }
    }
}