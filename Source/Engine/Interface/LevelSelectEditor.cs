/*
 * Author: Callen Betts (RiptideDev) 
 * Breif: Defines an inspector to display information about objects
 */

using ImGuiNET;
using Moonborne.Game.Objects;
using System.Reflection;
using System;
using Moonborne.Game.Room;
using Moonborne.Graphics.Camera;

namespace Moonborne.Engine.UI
{
    public static class LevelSelectEditor
    {
        public static string WindowName = "Room Manager";
        public static string NewRoomName = "NewRoom";

        /// <summary>
        /// Draw the inspector
        /// </summary>
        public static void Draw()
        {
            ImGui.Begin(WindowName);

            // Load a room by clicking on it
            foreach (var room in RoomManager.Rooms)
            {
                Room rm = room.Value;

                bool isSelected = ImGui.Selectable(rm.Name);

                if (isSelected)
                {
                    rm.Load(rm.Name);
                }
            }

            // Create new rooms in the room manager
            if (ImGui.BeginPopupContextItem("RoomContextMenu"))
            {
                // Add new room 
                if (ImGui.MenuItem("Create New Room"))
                {
                    if (!RoomManager.Rooms.ContainsKey(NewRoomName))
                    {
                        Room rm = new Room();
                        rm.Name = NewRoomName;
                        rm.Save(rm.Name);
                        RoomManager.Rooms.Add(rm.Name, rm);
                    }
                }

                // Input for room name
                ImGui.InputText("Room Name", ref NewRoomName, 20);

                ImGui.EndPopup();
            }

            // Trigger the context menu on right-click
            if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup("RoomContextMenu");
            }

            ImGui.End();
        }
    }
}
