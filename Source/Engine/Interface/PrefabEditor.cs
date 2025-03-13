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
using Moonborne.Graphics.Window;
using System.Text.Json;
using Moonborne.Engine.FileSystem;

namespace Moonborne.Engine.UI
{
    public static class PrefabEditor
    {
        public static bool IsActive;
        public static GameObject SelectedPrefab;
        public static List<GameObject> Prefabs = new List<GameObject>();
        public static float PreviewZoom = 1f;
        public static float PreviewZoomScroll = 0.25f;
        public static Vector2 PreviewOffset;

        /// <summary>
        /// Draws the preview of the prefab
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            // Draw background
            SpriteManager.DrawRectangle(0, 0, 320, 180, Color.Gray);

            // Zoom in and out and control preview position
            if (RoomEditor.HoveringOverGameWorld)
            {
                if (InputManager.MouseWheelDown())
                {
                    PreviewZoom -= PreviewZoomScroll;
                }
                if (InputManager.MouseWheelUp())
                {
                    PreviewZoom += PreviewZoomScroll;
                }
                PreviewZoom = Math.Clamp(PreviewZoom, 0.1f, 4f);

                // Pan the camera using right click
                if (InputManager.MouseMiddleDown())
                {
                    InputManager.SetMouseLocked(true);
                    PreviewOffset -= InputManager.GetMouseDeltaPosition() / 4;
                }
            }

            // Stop panning
            if (InputManager.MouseMiddleReleased())
            {
                InputManager.SetMouseLocked(false);
            }

            // Draw the prefab
            if (SelectedPrefab != null)
            {
                // Select this prefab
                Inspector.SelectedObject = SelectedPrefab;

                float drawX = (WindowManager.BaseViewportWidth/2);
                float drawY = (WindowManager.BaseViewportHeight/2);

                // Scale the preview 
                PreviewOffset.X = Math.Clamp(PreviewOffset.X, -100, 100);
                PreviewOffset.Y = Math.Clamp(PreviewOffset.Y, -100, 100);
                SelectedPrefab.Transform.Position = new Vector2(drawX, drawY)+PreviewOffset;

                // Draw the object
                Vector2 oldScale = SelectedPrefab.Transform.Scale;
                SelectedPrefab.Transform.Scale = PreviewZoom*oldScale;
                SelectedPrefab.Draw(spriteBatch);
                SelectedPrefab.Transform.Scale = oldScale;
                SelectedPrefab.DrawUI(spriteBatch);
            }
        }

        /// <summary>
        /// Saves all prefabs
        /// </summary>
        public static void SavePrefabs()
        {
            // Delete old prefabs
            string directory = Directory.GetCurrentDirectory() + "/Content/Data/Prefabs";

            // Save each prefab we currently have
            foreach (GameObject prefab in Prefabs)
            {
                GameObjectData data = GameObject.SaveData(prefab);
                FileHelper.DeleteFile(directory + "/" + data.Name + ".json");
                GameObject.SaveObjectDataToFile(data, "Content/Data/Prefabs");
            }

            Prefabs.Clear();

            LoadPrefabs();
        }

        /// <summary>
        /// Loads the prefabs into the list
        /// </summary>
        public static void LoadPrefabs()
        {
            // Get the directory
            string fileDirectory = $@"Content/Data/Prefabs";

            string[] files = Directory.GetFiles(fileDirectory);

            // Load all prefabs
            foreach (string file in files)
            {
                string json = File.ReadAllText(file);
                var prefabData = JsonSerializer.Deserialize<GameObjectData>(json);

                GameObject prefab = ObjectLibrary.CreateObject(prefabData.Name);
                GameObject.LoadData(prefab, prefabData);
                Prefabs.Add(prefab);
            }
        }

        /// <summary>
        /// Delete a selected prefab
        /// </summary>
        /// <param name="selectedPrefab"></param>
        public static void DeletePrefab(GameObject selectedPrefab)
        {
            if (selectedPrefab == null) 
                return;

            FileHelper.DeleteFile($"Content/Data/Prefabs/{selectedPrefab.DisplayName}.json");
            Prefabs.Remove(selectedPrefab);
            SelectedPrefab = null;
        }

        /// <summary>
        /// Reloads prefab data
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public static void ReloadPrefabs()
        {
            try
            {
                SavePrefabs();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }
    }
}
