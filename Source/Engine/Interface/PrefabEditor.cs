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
using Moonborne.Input;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Moonborne.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Game.Objects.Prefabs;

namespace Moonborne.Engine.UI
{
    public static class PrefabEditor
    {
        public static string WindowName = "Prefab";
        public static bool IsActive;
        public static GameObject SelectedPrefab;
        public static List<Prefab> Prefabs = new List<Prefab>();

        public static void Draw(SpriteBatch spriteBatch)
        {
            ImGui.Begin(WindowName);

            if (SelectedPrefab != null)
            {
                var imgPos = ImGui.GetItemRectMin(); // Top-left corner of the image
                var imgSize = ImGui.GetItemRectSize(); // Size of the image

                // Calculate the object's position in the ImGui Window
                float x = 1;
                float y = 1;

                float drawX = imgPos.X + x * RoomEditor.PreviewZoom;
                float drawY = imgPos.Y + y * RoomEditor.PreviewZoom;
                SelectedPrefab.Transform.Position = new Vector2(drawX, drawY);

                // Draw the object
                SelectedPrefab.Draw(spriteBatch);
            }

            ImGui.End();
        }
    }
}
