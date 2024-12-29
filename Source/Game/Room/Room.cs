﻿
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
                            TileSize = tilemap.tileSize,
                            TilesetName = tilemap.TilesetTextureName,
                            LayerName = tilemap.LayerName,
                            Depth = LayerManager.Layers[tilemap.LayerName].Depth,
                            Collideable = LayerManager.Layers[tilemap.LayerName].Collideable,
                            Visible = LayerManager.Layers[tilemap.LayerName].Visible,
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
                LayerManager.AddTilemapLayer(layer, tilemap, tilemapData.LayerName);

                // Set our currently selected tilemap
                RoomEditor.SelectedTilemap = tilemap;

                // Populate the grid with tile data
                foreach (var tile in tilemapData.Tiles)
                {
                    tilemap.grid[tile["x"], tile["y"]] = tile["tileId"];
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