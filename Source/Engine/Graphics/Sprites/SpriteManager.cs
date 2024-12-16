using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Moonborne.Graphics
{
    public static class SpriteManager
    {
        public static Dictionary<string, Texture2D> textures = new();
        public static Dictionary<string, Sprite> sprites = new();
        public static ContentManager content;
        public static GraphicsDevice graphicsDevice;

        /// <summary>
        /// Load and initialize all textures
        /// </summary>
        /// <param name="content"></param>
        public static void Initialize(ContentManager contentManager, GraphicsDevice device)
        {
            content = contentManager;
            graphicsDevice = device;
        }

        /// <summary>
        /// Loads all textures
        /// </summary>
        public static void LoadAllTextures()
        {
            // Path to the raw Textures folder
            string texturesDirectory = "Content/Textures";

            if (!Directory.Exists(texturesDirectory))
            {
                throw new DirectoryNotFoundException($"Textures directory not found at: {texturesDirectory}");
            }

            // Get all image files in the Textures folder
            string[] files = Directory.GetFiles(texturesDirectory, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                // Only process image files
                if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg"))
                {
                    // Extract the file name without the extension for use as a key
                    string textureName = Path.GetFileNameWithoutExtension(file);

                    // Load the texture from the file stream
                    using FileStream stream = new FileStream(file, FileMode.Open);
                    Texture2D texture = Texture2D.FromStream(graphicsDevice, stream);

                    // Add the texture to the dictionary
                    if (!textures.ContainsKey(textureName))
                    {
                        textures[textureName] = texture;
                        sprites[textureName] = new Sprite(texture);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve a texture given a name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(string name)
        {
            return textures.TryGetValue(name, out var texture) ? texture : null;
        }
    }
}