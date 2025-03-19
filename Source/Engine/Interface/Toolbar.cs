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
using Moonborne.Graphics.Window;
using Moonborne.Graphics;

namespace Moonborne.Engine.UI
{
    public static class Toolbar
    {
        public static string WindowName = "Toolbar";

        /// <summary>
        /// Draw play button, etc
        /// </summary>
        public static void DrawSecondLayer()
        {
            // Play button, scene tab
            ImGui.Begin("Game");
            IntPtr playIcon = SpriteManager.GetImGuiTexture("PlayIcon");
            IntPtr stopIcon = SpriteManager.GetImGuiTexture("StopIcon");
            IntPtr currentIcon = GameManager.Paused ? playIcon : stopIcon;
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(1, 1, 1, 0.2f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(1, 1, 1, 0.3f));

            if (ImGui.ImageButton(string.Empty, currentIcon, new System.Numerics.Vector2(48, 48)))
            {
                GameManager.ToggleGamemode();
            }
            ImGui.PopStyleColor(3);
            ImGui.End();
        }

        /// <summary>
        /// Draw the inspector
        /// </summary>
        public static void Draw()
        {
            // Toolbar Tab
            ImGui.Begin(WindowName, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.MenuBar);

            // 🔹 Begin a properly formatted menu bar
            if (ImGui.BeginMenuBar())
            {
                // 🟢 File Menu (Properly Separated)
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Save")) GameManager.Save();
                    if (ImGui.MenuItem("Package")) ExportGame(Directory.GetCurrentDirectory(), $"Desktop/Moonborne_Alpha.zip");
                    if (ImGui.MenuItem("Exit")) GameManager.Exit();
                    ImGui.EndMenu();
                }

                // 🟢 Edit Menu
                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Delete")) Inspector.DeleteSelectedObject();
                    if (ImGui.MenuItem("Undo (Ctrl+Z)")) { /* Undo logic */ }
                    if (ImGui.MenuItem("Redo (Ctrl+Y)")) { /* Redo logic */ }
                    ImGui.EndMenu();
                }

                // 🟢 Options Menu
                if (ImGui.BeginMenu("Options"))
                {
                    ImGui.Checkbox("Debug Mode", ref RoomEditor.DebugDraw);
                    if (ImGui.MenuItem("Fullscreen")) WindowManager.ToggleFullsceen();
                    ImGui.ShowFontSelector("Fonts");
                    ImGui.EndMenu();
                }

                // 🟢 Help Menu
                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.MenuItem("Discord"))
                    {
                        System.Diagnostics.Process.Start("explorer", "https://discord.gg/crwjFm7ka6");
                    }
                    ImGui.EndMenu();
                }

                ImGui.EndMenuBar(); // ✅ Ensures proper menu separation
            }

            ImGui.End(); // ✅ Ends the window
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
