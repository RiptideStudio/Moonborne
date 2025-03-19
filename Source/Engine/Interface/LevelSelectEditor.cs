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
        public static Room SelectedRoom = null;

        /// <summary>
        /// Draw the inspector
        /// </summary>
        public static void Draw()
        {

        }
    }
}
