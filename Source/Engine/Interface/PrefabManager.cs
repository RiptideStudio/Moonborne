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
using Newtonsoft.Json;
using Moonborne.Game.Assets;

namespace Moonborne.Engine.UI
{
    public static class PrefabManager
    {
        public static bool IsActive;
        public static Prefab SelectedPrefab;
        public static List<Prefab> Prefabs = new List<Prefab>();
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

            // Handle input like panning and moving
            HandleInput();  
        }

        private static void HandleInput()
        {
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
        }

        /// <summary>
        /// Load a prefab from a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Prefab LoadPrefab(string filePath)
        {
            string json = File.ReadAllText(filePath);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            Prefab prefab = JsonConvert.DeserializeObject<Prefab>(json, settings);
            Prefabs.Add(prefab);
            return prefab;
        }
    }
}
