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
using System.Xml.Linq;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Assets;

namespace Moonborne.Engine.UI
{
    public static class ContentDrawer
    {
        public static string WindowName = "Content Drawer";

        /// <summary>
        /// Render the content drawer
        /// </summary>
        public static void Draw()
        {
            ImGui.Begin(WindowName);

            // Show every asset in their respective folder
            AssetManager.ShowAssetBrowser();

            // Handle actions for right clicking

            // Display a list of all objects, and allow us to drag them into the game
            var list = ObjectLibrary.GetAllGameObjectNames();

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && ImGui.IsWindowHovered())
            {
                ImGui.OpenPopup("PrefabContextMenu");
            }

            // Click out of the prefab window
            if (ImGui.IsWindowHovered())
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) || ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    // Open prefab for edit
                }
            }

            // Opt to create a new prefab
            if (ImGui.BeginPopupContextItem("PrefabContextMenu"))
            {
                // Create a prefab
                if (ImGui.MenuItem("Create Prefab"))
                {
                    Prefab prefab = new Prefab("Test", "Prefabs");
                    AssetManager.Assets.Add(prefab);
                    PrefabManager.Prefabs.Add(prefab);
                }

                // Create a model
                if (ImGui.MenuItem("Create Model"))
                {
                    Prefab prefab = new Prefab("Tree", "Models");
                    AssetManager.Assets.Add(prefab);
                }

                // Delete a prefab
                if (ImGui.MenuItem("Delete Prefab"))
                {
                }
                ImGui.EndPopup();
            }

            ImGui.End();
        }
    }
}
