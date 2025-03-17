using ImGuiNET;
using Moonborne.Engine.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonborne.Game.Assets
{
    public static class AssetManager
    {
        public static List<Asset> Assets = new List<Asset>();
        public static string RootSavePath = @"Content/Assets";
        public static Asset SelectedAsset = null;
        public static Dictionary<string, List<Asset>> AssetsByFolder { get; private set; } = new Dictionary<string, List<Asset>>();
        private static string assetDirectory = @"Content/Assets/";

        /// <summary>
        /// Show all the assets n the browser
        /// </summary>
        public static void ShowAssetBrowser()
        {
            GetAssetsByFolder();

            foreach (var folder in AssetsByFolder)
            {
                if (ImGui.TreeNode(folder.Key))
                {
                    foreach (Asset asset in folder.Value)
                    {
                        bool selected = (SelectedAsset == asset);
                            
                        if (ImGui.Selectable(asset.Name,selected))
                        {
                            SelectedAsset = asset;
                        }
                    }
                    ImGui.TreePop();
                }
            }
        }

        /// <summary>
        /// Return a list of assets to render based on the folder we're in
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<Asset>> GetAssetsByFolder()
        {
            AssetsByFolder = Assets.GroupBy(a  => a.Folder).ToDictionary(g => g.Key, gc => gc.ToList());

            return AssetsByFolder;
        }

        /// <summary>
        /// Load all assets
        /// </summary>
        /// <summary>
        /// Load all assets from disk.
        /// </summary>
        public static void LoadAssets()
        {
            Console.WriteLine("[AssetManager] Loading assets...");

            if (!Directory.Exists(assetDirectory))
                Directory.CreateDirectory(assetDirectory);

            AssetsByFolder.Clear();

            // Load all files recursively
            foreach (var file in Directory.GetFiles(assetDirectory, "*.*", SearchOption.AllDirectories))
            {
                string extension = Path.GetExtension(file).ToLower();
                string folder = Path.GetRelativePath(assetDirectory, Path.GetDirectoryName(file));

                Asset asset = extension switch
                {
                    ".json" when file.Contains("Prefabs") => PrefabManager.LoadPrefab(file), // Prefab JSON handling
                    _ => null // Skip unknown files
                };

                if (asset != null)
                {
                    if (!AssetsByFolder.ContainsKey(folder))
                        AssetsByFolder[folder] = new List<Asset>();

                    AssetsByFolder[folder].Add(asset);
                }
            }

            Console.WriteLine($"[AssetManager] Loaded {AssetsByFolder.Count} asset folders.");
        }

        /// <summary>
        /// Save all assets to a registry file.
        /// </summary>
        public static void SaveAssets()
        {
            Console.WriteLine("[AssetManager] Saving asset registry...");

            foreach (var folder in AssetsByFolder)
            {
                string json = JsonConvert.SerializeObject(folder.Value, Formatting.Indented);
                File.WriteAllText(assetDirectory+"/"+folder.Key+".json", json);
            }

            Console.WriteLine("[AssetManager] Assets saved.");
        }
    }
}
