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
using MonoGame.Extended.Collisions.Layers;

namespace Moonborne.Engine.UI
{
    public static class LevelSelectEditor
    {
        public static string WindowName = "Room Manager";
        public static string NewRoomName = "NewRoom";
        public static bool isSelected = false;

        /// <summary>
        /// Draw the inspector
        /// </summary>
        public static void Draw()
        {
            ImGui.Begin(WindowName);
            ImGui.Text("Room Select");
            ImGui.Separator();

            // Load a room by clicking on it
            foreach (var room in RoomManager.Rooms)
            {
                Room rm = room.Value;

                // Check if this is the currently selected layer and mark it as selected
                isSelected = false;

                if (RoomEditor.CurrentRoom.Name == rm.Name)
                {
                    isSelected = true;
                }

                ImGui.Selectable(rm.Name, isSelected);

                if (ImGui.IsItemClicked())
                {
                    RoomManager.SetActiveRoom(rm);
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
                        RoomManager.Rooms.Add(rm.Name, rm);
                        RoomManager.SetActiveRoom(rm);
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
