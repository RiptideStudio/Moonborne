using ImGuiNET;
using Moonborne.Engine.Components;
using Moonborne.Engine.UI;
using Moonborne.Game.Objects;
using Moonborne.Game.Objects.Prefabs;
using Moonborne.Game.Room;
using Moonborne.Graphics;
using Moonborne.Input;
using Moonborne.UI.Dialogue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Moonborne.Game.Assets
{
    public static class AssetManager
    {
        public static List<Asset> Assets = new List<Asset>();
        public static Asset SelectedAsset = null;
        public static GameObject CreatedObject = null;
        public static Dictionary<string, List<Asset>> AssetsByFolder { get; private set; } = new Dictionary<string, List<Asset>>();
        private static string assetDirectory = @"Content/Assets/";
        private static string SelectedFolder = "";

        /// <summary>
        /// Show all the assets in the browser
        /// </summary>
        public static void ShowAssetBrowser()
        {
            // Update assets in the folder
            GetAssetsByFolder();

            // Display Folders as Horizontal Row with Icons
            foreach (var folder in AssetsByFolder)
            {
                ImGui.SameLine();

                ImGui.BeginGroup(); // Begin vertical stack

                // Folder Icon (Ensure it supports transparency)
                IntPtr folderIcon = SpriteManager.GetImGuiTexture("FolderClosed");

                if (SelectedFolder == folder.Key)
                {
                    folderIcon = SpriteManager.GetImGuiTexture("FolderOpen"); 
                }

                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(1, 1, 1, 0.2f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(1, 1, 1, 0.3f));
                float iconSize = 64;

                if (ImGui.ImageButton(folder.Key, folderIcon, new Vector2(iconSize, iconSize)))
                {
                    SelectedFolder = (SelectedFolder == folder.Key) ? null : folder.Key;
                }

                ImGui.PopStyleColor(3); // Removes the last 3 pushed styles

                // Center the text below the icon
                float textWidth = ImGui.CalcTextSize(folder.Key).X;
                float textOffset = 4+(iconSize - textWidth) * 0.5f;

                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + textOffset);
                ImGui.Text(folder.Key);

                ImGui.EndGroup(); // End vertical stack
            }

            ImGui.Separator();
            ImGui.NewLine();

            // If a folder is selected, show its assets in a horizontal drawer
            if (!string.IsNullOrEmpty(SelectedFolder) && AssetsByFolder.ContainsKey(SelectedFolder))
            {
                foreach (Asset asset in AssetsByFolder[SelectedFolder])
                {
                    ImGui.SameLine(); // Force assets to appear next to each other

                    ImGui.BeginGroup(); // Begin vertical stack

                    // Asset Icon (Ensure transparency works)
                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(1, 1, 1, 0.2f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(1, 1, 1, 0.3f));

                    SpriteTexture texture = SpriteManager.GetTexture(asset.Name);
                    IntPtr assetIcon = SpriteManager.GetImGuiTexture(asset.Name);
                    float iconSize = 64;
                    Vector2 finalSize = new Vector2(iconSize, iconSize);

                    if (texture != null)
                    {
                        assetIcon = texture.Icon;          
                    }

                    if (ImGui.ImageButton(asset.Name, assetIcon, finalSize))
                    {
                        SelectAsset(asset);
                    }
                    if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    {
                        asset.OnDoubleClick();
                    }

                    ImGui.PopStyleColor(3);

                    // Drag handling
                    DragAsset();

                    // Center the text below the icon
                    string displayName = asset.Name ?? "";
                    float textWidth = ImGui.CalcTextSize(string.IsNullOrEmpty(displayName) ? " " : displayName).X;
                    float textOffset = 4 + (iconSize - textWidth) * 0.5f;

                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + textOffset);
                    ImGui.Text(asset.Name);

                    ImGui.EndGroup(); // End vertical stack
                }
            }

            // Drop asset handle
            DropAsset();
        }

        /// <summary>
        /// Drag and drop operations on a select asset
        /// </summary>
        /// <param name="asset"></param>
        public static void DragAsset()
        {
            if (SelectedAsset is Prefab prefab && ImGui.BeginDragDropSource())
            {
                if (!RoomEditor.Dragging)
                {
                    Vector2 position = InputManager.MouseWorldCoords().ToNumerics();
                    Console.WriteLine($"Created {SelectedAsset.Name} at {position}");
                    CreatedObject = prefab.Instantiate(position);
                }
                RoomEditor.Dragging = true;
                ImGui.Text($"{prefab.Name}");
                ImGui.EndDragDropSource();
            }
        }

        /// <summary>
        /// Drop the selected asset
        /// </summary>
        public static void DropAsset()
        {
            // Instantiate prefab
            if (SelectedAsset is Prefab prefab)
            {
                if (InputManager.MouseLeftReleased() && RoomEditor.Dragging)
                {
                    CreatedObject = null;
                    RoomEditor.Dragging = false;
                    Inspector.SelectedObject = RoomEditor.selectedObject;
                }

                if (CreatedObject != null)
                {
                    Vector2 position = InputManager.MouseWorldCoords().ToNumerics();
                    position.X = ((int)(position.X + (RoomEditor.CellSize / 2)) / RoomEditor.CellSize) * RoomEditor.CellSize;
                    position.Y = ((int)(position.Y + (RoomEditor.CellSize / 2)) / RoomEditor.CellSize) * RoomEditor.CellSize;
                    CreatedObject.GetComponent<Transform>().Position = position;
                }
            }
        }

        /// <summary>
        /// Select an asset to inspect/edit
        /// </summary>
        /// <param name="asset"></param>
        public static void SelectAsset(Asset asset)
        {
            SelectedAsset = asset;
            Inspector.SelectedObject = SelectedAsset;
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
            Console.WriteLine("Loading assets...");

            if (!Directory.Exists(assetDirectory))
                Directory.CreateDirectory(assetDirectory);

            AssetsByFolder.Clear();

            // Load all assets recursively
            foreach (var file in Directory.GetFiles(assetDirectory, "*.*", SearchOption.AllDirectories))
            {
                string extension = Path.GetExtension(file).ToLower();
                string folder = Path.GetRelativePath(assetDirectory, Path.GetDirectoryName(file));

                Asset asset = extension switch
                {
                    ".json" when file.Contains("Prefabs") => PrefabManager.LoadPrefab(file),
                    ".json" when file.Contains("Dialogue") => DeserializeAsset<Dialogue>(file),
                    ".json" when file.Contains("Rooms") => DeserializeAsset<Room.Room>(file),
                    _ => null
                };

                if (asset != null)
                {
                    Assets.Add(asset);
                }
            }

            Console.WriteLine($"Loaded [{Assets.Count}] assets");
        }

        /// <summary>
        /// Deserialize an asset to a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeserializeAsset<T>(string fileName)
        {
            string json = File.ReadAllText(fileName);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            T obj = JsonConvert.DeserializeObject<T>(json, settings);


            return obj;
        }

        /// <summary>
        /// Save each asset to their respective folder
        /// </summary>
        public static void SaveAssets()
        {
            Console.WriteLine("Saving assets...");

            foreach (var folder in AssetsByFolder)
            {
                foreach (Asset asset in folder.Value)
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,  // Include type metadata in JSON
                        Formatting = Formatting.Indented
                    };

                    string json = JsonConvert.SerializeObject(asset, settings);
                    string directory = $"{assetDirectory}/{asset.Folder}";
                    string filePath = $"{directory}/{asset.Name}.json";

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.WriteAllText(filePath, json);
                }
            }

            Console.WriteLine("Assets saved!");
        }

        /// <summary>
        /// Get an asset 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Asset GetAsset(string folder, string name)
        {
            return Assets.FirstOrDefault(asset =>
                asset.Folder.Equals(folder, StringComparison.OrdinalIgnoreCase) &&
                asset.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get an asset of a type
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetAsset<T>(string name) where T : Asset
        {
            return Assets
                .OfType<T>()
                .FirstOrDefault(asset =>
                    asset.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
