/*
 * Author: Callen Betts (RiptideDev) 
 * Breif: Defines an inspector to display information about objects
 */

using ImGuiNET;
using Moonborne.Game.Objects;
using System.Reflection;
using System;
using Moonborne.Game.Room;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Moonborne.Graphics.Camera;

namespace Moonborne.Engine.UI
{
    public static class SettingsManager
    {
        public static string SavePath = @"Content/Config/Settings.ini";
        /// <summary>
        /// Saves settings
        /// </summary>
        public static void Save()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter($"{SavePath}", false))
                {
                    StringBuilder file = new StringBuilder();

                    // Save all specific values
                    file.AppendLine($"[RoomEditor]");
                    file.AppendLine($"CurrentRoom={RoomEditor.CurrentRoom.Name}");

                    writer.WriteLine(file.ToString());
                }
            }
            // If we fail to open the file
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Loads settings file
        /// </summary>
        public static void Load()
        {
            IniFile ini = new IniFile($"{SavePath}");
            string CurrentRoom = ini.GetValue("RoomEditor", "CurrentRoom");

            if (ini.IsValid())
            {
                RoomManager.SetActiveRoom(CurrentRoom);
            }
        }
    }
}
