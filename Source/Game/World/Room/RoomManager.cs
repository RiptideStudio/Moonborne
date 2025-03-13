
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Moonborne.Engine.Components;
using Moonborne.Engine.FileSystem;
using Moonborne.Engine.UI;
using Moonborne.Game.Objects;
using Moonborne.UI.Dialogue;
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
        /// 
        /// </summary>
        /// <param name="rm"></param>
        public static void SetActiveRoom(Room rm)
        {
            if (rm == null)
            {
                rm = GetDefaultRoom();
            }

            LayerManager.Clear();
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
            Rooms.TryGetValue(rm, out Room room);
            SetActiveRoom(room);
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