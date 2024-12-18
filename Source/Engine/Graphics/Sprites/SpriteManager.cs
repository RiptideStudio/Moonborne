using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ImGuiNET;

namespace Moonborne.Graphics
{
    public static class SpriteManager
    {
        public static Dictionary<string, Texture2D> textures = new();
        public static Dictionary<string, Sprite> sprites = new();
        public static ContentManager content;
        public static GraphicsDevice graphicsDevice;
        public static SpriteBatch UISpriteBatch;
        public static SpriteBatch spriteBatch;
        private static Dictionary<Texture2D, IntPtr> textureCache = new Dictionary<Texture2D, IntPtr>();

        /// <summary>
        /// Load and initialize all textures
        /// </summary>
        /// <param name="content"></param>
        public static void Initialize(ContentManager contentManager, GraphicsDevice device)
        {
            content = contentManager;
            graphicsDevice = device;
            UISpriteBatch = new SpriteBatch(graphicsDevice); // Sprite batch for UI
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

                        // Spritesheets should be uniform width and height
                        int maxFrames = texture.Width/texture.Height;
                        Sprite sprite = new Sprite(texture);

                        // Assuming same dimensions for width and height by default
                        sprite.MaxFrames = maxFrames;
                        sprite.FrameWidth = texture.Height;
                        sprite.FrameHeight = texture.Height;

                        sprites[textureName] = sprite;
                    }
                }
            }
        }

        /// <summary>
        /// Draws a sprite given a texture name - this is very powerful and can be called from anywhere
        /// </summary>
        /// <param name="sprite"></param>
        public static void DrawSprite(string spriteName, int frame, Vector2 position, Vector2 scale, float rotation, bool ui=false)
        {
            Sprite sprite = GetSprite(spriteName);

            // Draw a sprite given parameters, and use UI positioning if defined as such
            if (sprite != null)
            {
                if (ui)
                {
                    sprite.Draw(UISpriteBatch, frame, position, scale, rotation);
                }
                else
                {
                    sprite.Draw(spriteBatch, frame, position, scale, rotation);
                }
            }
        }

        /// Draw text with optional scale, rotation, and UI overlay handling.
        /// </summary>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The position to draw the text.</param>
        /// <param name="scale">The scaling factor for the text.</param>
        /// <param name="rotation">The rotation of the text in radians.</param>
        /// <param name="ui">If true, the text is drawn without transformations (for UI elements).</param>
        public static void DrawText(string text, Vector2 position, Vector2 scale, float rotation, SpriteFont font)
        {
            if (string.IsNullOrEmpty(text) || font == null)
                return;

            // Measure the text size for scaling and centering purposes
            Vector2 textSize = font.MeasureString(text);

            // Apply transformation matrix for UI or world space
            Vector2 origin = textSize / 2f; // Origin (centered)
            Vector2 scaledSize = textSize * scale; // Scale text size

            // Draw text using spriteBatch
            spriteBatch.DrawString(
                font,                    // The font to use
                text,                    // The string to draw
                position,                // The position in world or screen coordinates
                Color.White);            // Text color
        }

        /// <summary>
        /// Draws a sprite given a texture name - this is very powerful and can be called from anywhere
        /// </summary>
        /// <param name="sprite"></param>
        public static void DrawTileFromTileset(string tileSet, int tileID, int tileSize, Vector2 position, Vector2 scale)
        {
            Sprite sprite = GetSprite(tileSet);

            // Draw a tile given a tileset and tile ID. Tile ID is the frame of the tile we want to draw
            if (sprite != null)
            {
                sprite.FrameWidth = tileSize;
                sprite.FrameHeight = tileSize;
                sprite.MaxFrames = (sprite.Texture.Width/ tileSize) * tileSize;
                sprite.Draw(spriteBatch, tileID, position, scale);
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
        
        /// <summary>
        /// Retrieve a texture given a name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Sprite GetSprite(string name)
        {
            return sprites.TryGetValue(name, out var sprite) ? sprite : null;
        }
    }
}