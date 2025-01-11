
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Moonborne.Engine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Moonborne.Game.Room
{
    public static class RoomManager
    {
        public static Room CurrentRoom;
        public static Dictionary<string, Room> Rooms = new Dictionary<string, Room>();

        /// <summary>
        /// Loads all rooms at the start of the game. Stores them into a map for access
        /// </summary>
        public static void LoadRooms(GraphicsDevice graphicsDevice, MGame game)
        {
            string contentFolderPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Content\Rooms"));

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
            LevelSelectEditor.isSelected = true;
            Console.WriteLine($"Switched to room {rm.Name}");
        }
    }
}