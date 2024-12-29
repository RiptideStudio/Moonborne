/*
 * Author: Callen Betts (RiptideDev) 
 * Breif: Defines an inspector to display information about objects
 */

using ImGuiNET;
using Moonborne.Game.Objects;
using System.Reflection;
using System;
using Moonborne.Game.Room;

namespace Moonborne.Engine.UI
{
    public static class SettingsEditor
    {
        public static string WindowName = "Game Settings";

        /// <summary>
        /// Draw the inspector
        /// </summary>
        public static void Draw()
        {
            ImGui.Begin(WindowName);

            if (ImGui.Selectable("Save"))
            {
                RoomEditor.CurrentRoom.Save(RoomEditor.CurrentRoom.Name);
            }
            if (ImGui.Selectable("Load"))
            {
                RoomEditor.CurrentRoom.Load(RoomEditor.CurrentRoom.Name);
            }

            ImGui.Checkbox("Debug Mode", ref GameManager.DebugMode);

            ImGui.End();
        }
    }
}
