/*
 * Author: Callen Betts (RiptideDev) 
 * Breif: Defines an inspector to display information about objects
 */

using ImGuiNET;
using Moonborne.Game.Objects;
using System.Reflection;
using System;
using Moonborne.Game.Room;
using System.Numerics;
using System.IO.Compression;
using System.IO;

namespace Moonborne.Engine.UI
{
    public static class Toolbar
    {
        public static string WindowName = "Toolbar";

        /// <summary>
        /// Draw the inspector
        /// </summary>
        public static void Draw()
        {
            ImGui.BeginMainMenuBar();

            // Play/ Stop button
            string playButtonName = "Play";

            if (!GameManager.Paused)
            {
                playButtonName = "Stop";
            }

            if (ImGui.Button(playButtonName))
            {
                GameManager.ToggleGamemode();
            }

            // File menu options
            if (ImGui.BeginMenu("File"))
            {
                // Save Current room
                if (ImGui.MenuItem("Save"))
                {
                    GameManager.Save();
                }
                // Export
                if (ImGui.MenuItem("Package"))
                {
                    ExportGame(Directory.GetCurrentDirectory(), $"Desktop/Moonborne_Alpha.zip");
                }
                // Exit engine
                if (ImGui.MenuItem("Exit"))
                {
                    GameManager.Exit();
                }
                ImGui.EndMenu();
            }
            // Edit
            if (ImGui.BeginMenu("Edit"))
            {
                // Delete selected object
                if (ImGui.MenuItem("Delete"))
                {
                    Inspector.DeleteSelectedObject();
                }
                // Undo
                if (ImGui.MenuItem("Undo (Ctrl+Z)"))
                {
                }
                // Redo
                if (ImGui.MenuItem("Redo (Ctrl+Y)"))
                {
                }
                ImGui.EndMenu();
            }
            // Global Options
            if (ImGui.BeginMenu("Options"))
            {
                // Debug mode
                ImGui.Checkbox("Debug Mode", ref RoomEditor.DebugDraw);
                ImGui.ShowFontSelector("Fonts");

                ImGui.EndMenu();
            }
            // Help
            if (ImGui.BeginMenu("Help"))
            {
                // Debug mode
                ImGui.Text("You're on your own");
                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }

        /// <summary>
        /// Creates a zip file from a directory, placing it in a target directory
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="targetDirectory"></param>
        public static void CreateZipFromDirectory(string directory, string targetDirectory)
        {
            string finalDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\" + targetDirectory));

            try
            {
                ZipFile.CreateFromDirectory(directory, finalDirectory);
                Console.WriteLine("Build success!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Build failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Make a build of the game for windows
        /// </summary>
        public static void ExportGame(string directory, string targetDirectory)
        {
            Console.WriteLine($"Building game to {targetDirectory}...");
            CreateZipFromDirectory(directory,targetDirectory);
        }
    }
}
