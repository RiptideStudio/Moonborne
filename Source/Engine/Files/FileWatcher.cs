
using Microsoft.Xna.Framework;
using System.IO;
using System;
using Moonborne.Graphics;

namespace Moonborne.Engine.Files
{
    public class GameWatcher
    {
        private static FileSystemWatcher watcher;
        public static bool Reloading = false;

        /// <summary>
        /// Initialize the watcher
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void StartWatching(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"[ERROR] Directory not found: {directoryPath}");
                return;
            }

            watcher = new FileSystemWatcher(directoryPath)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime,
                Filter = "*.*", // Watch all file types (restrict to *.png, *.jpg if needed)
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            // Add event handlers for certain changes
            watcher.Changed += OnTextureChanged;
        }

        /// <summary>
        /// Detect changes when a texture is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTextureChanged(object sender, FileSystemEventArgs e)
        {
            if (!Reloading)
            {
                Console.WriteLine($"[INFO] Texture change detected: {e.Name} ({e.ChangeType})");
                Reloading = true;
            }
        }

        /// <summary>
        /// Queue filechange callbacks so we don't reload at bad times
        /// </summary>
        public static void Update()
        {
            if (Reloading)
            {
                SpriteManager.ReloadTextures();
                Reloading = false;
            }
        }

        /// <summary>
        /// Stop watching
        /// </summary>
        public static void StopWatching()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
    }
}
