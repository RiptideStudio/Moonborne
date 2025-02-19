﻿
using FMOD;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Tiled;
using Moonborne.Engine.Components;
using Moonborne.Engine.UI;
using Moonborne.Game.Objects;
using Moonborne.Game.Objects.Prefabs;
using Moonborne.Graphics;
using Moonborne.Graphics.Camera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Moonborne.Game.Room
{
    public class RoomData
    {
        public string RoomName { get; set; }
        public List<TilemapData> Tilemaps { get; set; }
        public List<GameObjectWorldData> Objects { get; set; }
        public string SelectedLayer { get; set; }
        public int SelectedObject { get; set; }
        public int SelectedTile { get; set; }
    }

    public class VariableData
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string Type { get; set; }
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
                    Layer layer = new Layer(tilemapData.Depth, () => Camera.TransformMatrix, LayerType.Tile);
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

            try
            {
                // Reconstruct objects in each layer
                if (roomData.Objects != null)
                {
                    foreach (var objectData in roomData.Objects)
                    {
                        Vector2 position = new Vector2(objectData.PositionX, objectData.PositionY);
                        ObjectLibrary.CreatePrefab(objectData.TypeName, objectData.Name, position, objectData.LayerName);
                    }
                }
            }
            catch (Exception ex)
            {
                // If failed, throw an error
                Console.WriteLine(ex.ToString());
            }


            // Select the old selected layers and object if applicable
            RoomEditor.SelectLayer(LayerManager.GetLayer(roomData.SelectedLayer));
            Inspector.SelectedObject = LayerManager.GetInstance(roomData.SelectedObject);

            Console.WriteLine($"Loaded Room '{name}'");
        }

        /// <summary>
        /// Load an individual property into an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        public static void LoadProperty(object obj, VariableData property)
        {
            var key = property.Name;
            bool propertyExists = false;

            Type type = obj.GetType();
            PropertyInfo prop = type.GetProperty(property.Name);

            // Get the game object
            GameObject gameObject = (GameObject)obj;

            // Check for a valid property
            if (prop != null)
            {
                propertyExists = true;
            }
            else
            {
                // If not found in parent check all components
                foreach (ObjectComponent component in gameObject.Components)
                {
                    type = component.GetType();
                    prop = type.GetProperty(property.Name);

                    if (prop != null)
                    {
                        propertyExists = true;
                        obj = component;
                        break;
                    }
                }
            }

            // If no property exists do not set
            if (!propertyExists)
                return;

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
            else if (prop.PropertyType == typeof(string))
            {
                string fullString = property.Value.ToString();
                string value = string.Empty;
                foreach (char character in fullString)
                {
                    if (character == '\0')
                        break;

                    value += character;
                }

                prop.SetValue(obj, value, null);
            }
            else if (property.Type == "Vector")
            {
                Vector2 vector = new Vector2(property.X, property.Y);
                prop.SetValue(obj, vector, null);
            }
            else if (property.Type == "SpriteTexture")
            {
                if (property.Value != null)
                {
                    SpriteTexture tex = SpriteManager.GetTexture(property.Value.ToString());
                    prop.SetValue(obj, tex, null);
                }
            }
        }
    }
}