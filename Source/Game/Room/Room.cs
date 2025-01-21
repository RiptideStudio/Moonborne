
using FMOD;
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
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Moonborne.Game.Room
{
    public class RoomData
    {
        public string RoomName { get; set; }
        public List<TilemapData> Tilemaps { get; set; }
        public List<GameObjectData> Objects { get; set; }
    }

    public class VariableData
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }


    public class Room
    {
        public string Name { get; set; } = "Room";

        /// <summary>
        /// Save a room to a json file made of multiple tilemaps
        /// </summary>
        /// <param name="name"></param>
        /// <param name="overridePath"></param>
        public void Save(string name, string overridePath = null)
        {
            RoomManager.SaveRoom(name, overridePath);
            Console.WriteLine($"Saved Room '{name}'");
        }

        /// <summary>
        /// Load a room from multiple tilemaps
        /// </summary>
        /// <param name="name"></param>
        public void Load(string name, string overridePath=null)
        {
            string contentFolderPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"Content\Rooms"));

            if (overridePath != null)
            {
                contentFolderPath = overridePath;
            }

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
                    LayerManager.AddTilemapLayer(layer, tilemap, tilemapData.LayerName);

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
                    layer.Depth = objectData.Depth;
                    LayerManager.AddLayer(layer, objectData.LayerName);

                    Vector2 position = new Vector2(objectData.PositionX, objectData.PositionY);
                    GameObject obj = ObjectLibrary.CreateObject(objectData.Name, position, objectData.LayerName);

                    // Load properties
                    foreach (var property in objectData.Properties)
                    {
                        var key = property.Name;

                        Type type = obj.GetType();

                        PropertyInfo prop = type.GetProperty(property.Name);

                        // Don't set wrong property
                        if (prop == null || prop.Name != key)
                            continue;

                        // Hard checks for each type
                        if (prop.PropertyType == typeof(float))
                        {
                            var value = float.Parse(property.Value.ToString());
                            prop.SetValue(obj, value, null);
                        }
                        else if (prop.PropertyType == typeof(int))
                        {
                            int value = int.Parse(property.Value.ToString());
                            prop.SetValue(obj, value, null);
                        }                        
                        else if (prop.PropertyType == typeof(bool))
                        {
                            bool value = bool.Parse(property.Value.ToString());
                            prop.SetValue(obj, value, null);
                        }
                    }
                }
            }

            Console.WriteLine($"Loaded Room '{name}'");
        }
    }
}