
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Tiled;
using Moonborne.Game.Objects;
using Moonborne.Graphics;
using Moonborne.Graphics.Camera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;

namespace Moonborne.Game.Room
{
    public class RoomData
    {
        public string RoomName { get; set; }
        public List<TilemapData> Tilemaps { get; set; }
        public List<GameObjectData> Objects { get; set; }
    }

    public class Room
    {
        public string Name { get; set; } = "Room";

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
            var objects = new List<object>();

            // Iterate over all layers in the LayerManager
            foreach (var layer in LayerManager.Layers)
            {
                if (layer.Value.Locked)
                    continue;

                if (layer.Value.Type == LayerType.Tile) // Only process Tile layers
                {
                    foreach (var tilemap in layer.Value.Tilemaps) // Handle multiple tilemaps in the layer
                    {
                        var tiles = new List<Dictionary<string, int>>();

                        // Save each tile
                        foreach (var tile in tilemap.TileList)
                        {
                            tiles.Add(new Dictionary<string, int>
                            {
                                { "x", tile.Value.x },
                                { "y", tile.Value.y },
                                { "tileId", tile.Value.CellData },
                                { "tileHeight", layer.Value.Height }
                            });
                        }

                        // Add this tilemap's data to the tilemaps list
                        tilemaps.Add(new
                        {
                            TileSize = tilemap.tileSize,
                            TilesetName = tilemap.TilesetTextureName,
                            Height = LayerManager.Layers[tilemap.LayerName].Height,
                            LayerName = tilemap.LayerName,
                            Depth = LayerManager.Layers[tilemap.LayerName].Depth,
                            Collideable = LayerManager.Layers[tilemap.LayerName].Collideable,
                            Visible = LayerManager.Layers[tilemap.LayerName].Visible,
                            IsTransitionLayer = LayerManager.Layers[tilemap.LayerName].IsTransitionLayer,
                            Tiles = tiles
                        });
                    }
                }
                else if (layer.Value.Type == LayerType.Object)
                {
                    // Save objects on the layer
                    foreach (var obj in layer.Value.Objects)
                    {
                        objects.Add(new
                        {
                            PositionX = obj.Position.X,
                            PositionY = obj.Position.Y,
                            Name = obj.GetType().Name,
                            LayerName = layer.Value.Name
                        });
                    }
                }
            }

            // Create a JSON object for the room
            var roomData = new
            {
                RoomName = name,
                Tilemaps = tilemaps,
                Objects = objects
            };

            // Serialize the room data to JSON
            string json = JsonSerializer.Serialize(roomData, new JsonSerializerOptions { WriteIndented = true });

            // Write to file
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Saved Room '{name}' to {filePath}");
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
                return;
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
            if (roomData.Tilemaps != null)
            {
                foreach (var tilemapData in roomData.Tilemaps)
                {
                    // Create a new Tilemap instance
                    Tilemap tilemap = new Tilemap(
                        tilemapData.TilesetName,
                        new int[100, 100],
                        tilemapData.TileSize,
                        tilemapData.LayerName
                    );

                    // Reconstruct layers
                    Layer layer = new Layer(tilemapData.Depth, () => Camera.Transform, LayerType.Tile);
                    layer.Depth = tilemapData.Depth;
                    layer.Visible = tilemapData.Visible;
                    layer.Collideable = tilemapData.Collideable;
                    layer.Height = tilemapData.Height;
                    layer.IsTransitionLayer = tilemapData.IsTransitionLayer;
                    LayerManager.AddTilemapLayer(layer, tilemap, tilemapData.LayerName);
                    
                    // Set our currently selected tilemap
                    RoomEditor.SelectedTilemap = tilemap;

                    // Populate the grid with tile data
                    foreach (var tile in tilemapData.Tiles)
                    {
                        int gridX = tile["x"];
                        int gridY = tile["y"];
                        int tileId = tile["tileId"];
                        int tileHeight = tile["tileHeight"];

                        // Compute the unique key for the tile
                        int tileKey = gridX + gridY * 100;

                        // Add the tile to the TileList dictionary
                        tilemap.TileList[tileKey] = new Tile(gridX, gridY, tileId, tileHeight);

                        // Update the grid with the tile ID
                        tilemap.grid[gridX, gridY] = tileId;
                    }
                }
            }

            // Reconstruct objects in each layer
            if (roomData.Objects != null) 
            {
                foreach (var objectData in roomData.Objects)
                {
                    Layer layer = new Layer(1, () => Camera.Transform, LayerType.Object);
                    LayerManager.AddLayer(layer, objectData.LayerName);

                    Vector2 position = new Vector2(objectData.PositionX, objectData.PositionY);
                    ObjectLibrary.CreateObject(objectData.Name, position, objectData.LayerName);
                }
            }

            Console.WriteLine($"Loaded Room '{name}'");
        }
    }
}