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
using Moonborne.Engine.Components;
using Moonborne.UI.Dialogue;

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
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && ImGui.IsWindowHovered())
            {
                ImGui.OpenPopup("PrefabContextMenu");
            }

            // Opt to create a new prefab
            if (ImGui.BeginPopupContextItem("PrefabContextMenu"))
            {
                // Create a prefab
                if (ImGui.MenuItem("Create Prefab"))
                {
                    Prefab prefab = new Prefab("GameObject", "Prefabs");
                    AssetManager.Assets.Add(prefab);
                    PrefabManager.Prefabs.Add(prefab);
                    prefab.Components.Add(new Transform());
                }

                // Create a dialogue object
                if (ImGui.MenuItem("Create Dialogue"))
                {
                    Dialogue dialogue = new Dialogue("Dialogue_1", "Dialogue");
                    AssetManager.Assets.Add(dialogue);
                }

                // Delete an asset
                if (ImGui.MenuItem("Delete Asset"))
                {
                    AssetManager.Assets.Remove(AssetManager.SelectedAsset);
                }
                // Create a folder
                if (ImGui.MenuItem("Create New Folder"))
                {

                }

                // Delete a folder
                if (ImGui.MenuItem("Delete Folder"))
                {

                }
                ImGui.EndPopup();
            }

            ImGui.End();
        }
    }
}
