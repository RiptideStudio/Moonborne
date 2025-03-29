/**
 * @file ContentDrawer.cs
 * @brief Defines a UI drawer for displaying and managing game content assets
 * @author Callen Betts (RiptideDev)
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
using Moonborne.Game.Behaviors;

namespace Moonborne.Engine.UI
{
    /**
     * @brief Static class that handles drawing and managing the content browser window
     */
    public static class ContentDrawer
    {
        /** @brief The title displayed in the content drawer window */
        public static string WindowName = "Content Drawer";

        /**
         * @brief Renders the content drawer window and handles asset management operations
         *
         * Displays the asset browser and provides context menu options for:
         * - Creating folders
         * - Creating prefabs
         * - Creating dialogue assets
         * - Creating room assets
         * - Creating behavior trees
         * - Deleting assets and folders
         */
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

            // Context menu for asset operations
            if (ImGui.BeginPopupContextItem("PrefabContextMenu"))
            {
                // Folder operations
                if (ImGui.MenuItem("Create Folder"))
                {
                    AssetManager.AssetsByFolder.Add("New Folder", new List<Asset>());
                }
                
                // Asset creation operations
                CreateAssetMenuItem<Prefab>("Create Prefab", "GameObject", 
                    prefab => {
                        PrefabManager.Prefabs.Add(prefab);
                        prefab.Components.Add(new Transform());
                    });
                
                CreateAssetMenuItem<Dialogue>("Create Dialogue", "Dialogue_1");
                CreateAssetMenuItem<Room>("Create Room", "Room_1");
                CreateAssetMenuItem<BehaviorTreeAsset>("Create Behavior Tree", "BehaviorTree_1");

                // Delete operations
                if (ImGui.MenuItem("Delete Asset"))
                {
                    AssetManager.DeleteAsset(AssetManager.SelectedAsset);
                }
                
                if (ImGui.MenuItem("Delete Folder"))
                {

                }
                ImGui.EndPopup();
            }

            ImGui.End();
        }
        
        /**
         * @brief Helper method to create menu items for different asset types
         * @param menuName The name to display in the menu
         * @param assetName The default name for the new asset
         * @param postProcess Optional action to perform after asset creation
         */
        private static void CreateAssetMenuItem<T>(string menuName, string assetName, Action<T> postProcess = null) where T : Asset
        {
            if (ImGui.MenuItem(menuName))
            {
                T asset = (T)Activator.CreateInstance(typeof(T), assetName, AssetManager.SelectedFolder);
                AssetManager.AddAsset(asset);
                postProcess?.Invoke(asset);
            }
        }
    }
}
