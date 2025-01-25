
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Moonborne.Engine.Components;
using Moonborne.Engine.FileSystem;
using Moonborne.Engine.UI;
using Moonborne.Game.Objects;
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
            // Re-load everything in the room
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
            var objects = new List<GameObjectWorldData>();

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
                            Tiles = tiles
                        });
                    }
                }
                else if (layer.Value.Type == LayerType.Object)
                {
                    // Save objects on the layer
                    foreach (var obj in layer.Value.Objects)
                    {
                        // Add the data to the json object
                        GameObjectWorldData data = new GameObjectWorldData();
                        data.PositionX = obj.Transform.Position.X;
                        data.PositionY = obj.Transform.Position.Y;
                        data.Name = obj.DisplayName;
                        data.TypeName = obj.GetType().Name;
                        data.LayerName = layer.Value.Name;
                        data.InstanceID = obj.InstanceID;
                        objects.Add(data);
                    }
                }
            }

            // Create a JSON object for the room
            var roomData = new
            {
                RoomName = saveName,
                Tilemaps = tilemaps,
                Objects = objects,
                SelectedLayer = Layer.GetName(Inspector.SelectedLayer),
                SelectedObject = GameObject.GetID(Inspector.SelectedObject),
                SelectedTile = Layer.GetSelectedTileID()
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
            LayerManager.Clear();
            rm.Load(rm.Name);
            RoomEditor.CurrentRoom = rm;
            CurrentRoom = rm;
            LevelSelectEditor.isSelected = true;
        }

        /// <summary>
        /// Target room by string
        /// </summary>
        /// <param name="rm"></param>
        public static void SetActiveRoom(string rm)
        {
            if (!Rooms.ContainsKey(rm))
                return;

            Room targetRoom = Rooms[rm];

            SetActiveRoom(targetRoom);
        }

        /// <summary>
        /// Returns a default room if it exists
        /// </summary>
        /// <returns></returns>
        public static Room GetDefaultRoom()
        {
            // If we have no room, make a new one
            if (Rooms.Count == 0)
                return CreateRoom("Empty");

            // Return the first room in the list by default
            return Rooms.First().Value;
        }

        /// <summary>
        /// Deletes a room given the object
        /// </summary>
        /// <param name="currentRoom"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void DeleteRoom(Room currentRoom)
        {
            if (currentRoom == null)
                return;

            // Find the room and delete it
            string filePath = @$"Content/Rooms/{currentRoom.Name}.json";
            FileHelper.DeleteFile(filePath);

            // Remove it from the room list
            Rooms.Remove(currentRoom.Name);

            // Transition to a valid room
            SetActiveRoom(GetDefaultRoom());

            // Log success
            Console.WriteLine($"Deleted room at {filePath}");
        }

        /// <summary>
        /// Creates a new room given a name
        /// </summary>
        /// <param name="newRoomName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Room CreateRoom(string newRoomName)
        {
            Room rm = new Room();
            rm.Name = newRoomName;
            Rooms.Add(rm.Name, rm);
            return rm;
        }
    }
}