
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Moonborne.Engine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Moonborne.Game.Room
{
    public static class RoomManager
    {
        public static Room CurrentRoom;
        public static Dictionary<string, Room> Rooms = new Dictionary<string, Room>();
        public static int TempIteration = 0;

        /// <summary>
        /// Save a snapshot the current room
        /// </summary>
        public static void SaveSnapshot()
        {
            if (CurrentRoom != null)
            {
                TempIteration++;
                SaveRoom(CurrentRoom.Name + $"tmp_{TempIteration}",@"Content/Temp",CurrentRoom.Name);
            }
        }

        /// <summary>
        /// Load a snapshot
        /// </summary>
        public static void LoadSnapshot(int temp)
        {
            if (CurrentRoom != null)
            {
                CurrentRoom.Load(CurrentRoom.Name+$"tmp_{temp}", @"Content/Temp");
            }
        }

        /// <summary>
        /// Undo and load older state
        /// </summary>
        public static void Undo()
        {
            if (TempIteration <= 0)
                return;

            TempIteration--;
            LoadSnapshot(TempIteration);
        }

        /// <summary>
        /// Redo an action
        /// </summary>
        public static void Redo()
        {
            if (File.Exists($"Content/Temp/{CurrentRoom.Name}tmp_{TempIteration}"))
                return;

            LoadSnapshot(TempIteration++);
        }

        /// <summary>
        /// Save a room to a destination
        /// </summary>
        /// <param name="name"></param>
        /// <param name="overridePath"></param>
        public static void SaveRoom(string name, string overridePath = null, string saveName = null)
        {
            string contentFolderPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"Content\Rooms"));

            if (overridePath != null)
            {
                contentFolderPath = overridePath;
            }
            if (saveName == null)
            {
                saveName = name;
            }

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
                            Depth = LayerManager.Layers[tilemap.LayerName].Depth,
                            LayerName = tilemap.LayerName,
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
                        // Add each serializable property of the object to the property list
                        PropertyInfo[] properties = obj.GetType().GetProperties();
                        var objectProperties = new List<object>();

                        foreach (var property in properties)
                        {
                            // objectProperties.Add(property.GetValue(obj));
                        }

                        // Add the data to the json object
                        objects.Add(new
                        {
                            PositionX = obj.Position.X,
                            PositionY = obj.Position.Y,
                            Name = obj.GetType().Name,
                            Depth = obj.Layer.Depth,
                            LayerName = layer.Value.Name,
                            Properties = objectProperties
                        });
                    }
                }
            }

            // Create a JSON object for the room
            var roomData = new
            {
                RoomName = saveName,
                Tilemaps = tilemaps,
                Objects = objects
            };

            // Serialize the room data to JSON
            string json = JsonSerializer.Serialize(roomData, new JsonSerializerOptions { WriteIndented = true });

            // Write to file
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads all rooms at the start of the game. Stores them into a map for access
        /// </summary>
        public static void LoadRooms(GraphicsDevice graphicsDevice, MGame game)
        {
            string contentFolderPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"Content\Rooms"));

            if (!Directory.Exists(contentFolderPath))
            {
                return;
            }

            string[] files = Directory.GetFiles(contentFolderPath);

            foreach (string file in files)
            {
                if (file.EndsWith(".json"))
                {
                    Room rm = new Room();
                    rm.Name = Path.GetFileNameWithoutExtension(file);
                    Rooms.Add(rm.Name, rm);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rm"></param>
        public static void SetActiveRoom(Room rm)
        {
            RoomEditor.CurrentRoom.Save(RoomEditor.CurrentRoom.Name);
            LayerManager.Clear();
            rm.Load(rm.Name);
            RoomEditor.CurrentRoom = rm;
            CurrentRoom = rm;
            LevelSelectEditor.isSelected = true;
            Console.WriteLine($"Switched to room {rm.Name}");
        }
    }
}