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

namespace Moonborne.Engine.UI
{
    public static class ConsoleEditor
    {
        public static string WindowName = "Console";
        private static StringWriter consoleOutput = new StringWriter();
        private static List<string> logs = new List<string>(); // Stores logs for display
        private static bool newLogAdded = false;

        /// <summary>
        /// Initializes the custom console output.
        /// </summary>
        public static void Initialize()
        {
            Console.SetOut(consoleOutput); // Redirect Console.WriteLine output to our StringWriter
        }

        /// <summary>
        /// Draws the console output into an ImGui window
        /// </summary>
        public static void Draw()
        {
            ImGui.Begin(WindowName);

            string newOutput = consoleOutput.ToString();
            if (!string.IsNullOrWhiteSpace(newOutput))
            {
                logs.AddRange(newOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                consoleOutput.GetStringBuilder().Clear(); // Clear the captured output
                newLogAdded = true;
            }

            // Log the text
            foreach (var log in logs)
            {
                ImGui.TextWrapped(log);
            }

            // Auto-scroll to the bottom if new logs are added
            if (newLogAdded)
            {
                ImGui.SetScrollHereY(1.0f);
                newLogAdded = false;
            }

            ImGui.End();
        }
    }
}
