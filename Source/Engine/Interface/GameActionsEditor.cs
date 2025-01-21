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
    public static class GameActionsEditor
    {
        public static string WindowName = "Game";

        /// <summary>
        /// Draw the inspector
        /// </summary>
        public static void Draw()
        {
            ImGui.Begin(WindowName);

            ImGui.End();
        }
    }
}
