using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ImGuiNET;
using System.Reflection.Metadata;

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
        public static SpriteFont GameFont;
        public static Texture2D PixelTexture;

        /// <summary>
        /// Load and initialize all textures
        /// </summary>
        /// <param name="content"></param>
        public static void Initialize(ContentManager contentManager, GraphicsDevice device)
        {
            content = contentManager;
            graphicsDevice = device;
            UISpriteBatch = new SpriteBatch(graphicsDevice); // Sprite batch for UI
            PixelTexture = Create1x1Texture(Color.White);
            GameFont = content.Load<SpriteFont>("Fonts/GameFont");
        }

        /// <summary>
        /// Creates a 1x1 texture for drawing primitives
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Texture2D Create1x1Texture(Color color)
        {
            // Create a new 1x1 texture
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);

            // Set the pixel data to the specified color
            texture.SetData(new[] { color });

            return texture;
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
        public static void DrawText(string text, Vector2 position, Vector2 scale, float rotation, Color color, int maxWidth = 1000)
        {
            // Check for invalid input
            if (string.IsNullOrEmpty(text) || GameFont == null)
                return;

            // Word-wrapping logic
            string[] words = text.Split(' '); // Split text by spaces
            List<string> lines = new List<string>();

            string currentLine = "";
            foreach (var word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";

                // Measure the line width
                if (GameFont.MeasureString(testLine).X * scale.X > maxWidth)
                {
                    // Add the current line to the list and start a new line
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    // Append the word to the current line
                    currentLine = testLine;
                }
            }

            // Add the last line
            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }

            // Draw each line with a vertical offset
            Vector2 linePosition = position;
            foreach (var line in lines)
            {
                spriteBatch.DrawString(GameFont, line, linePosition, color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0);
                linePosition.Y += GameFont.LineSpacing * scale.Y; // Move down by line spacing
            }
        }

        /// <summary>
        /// Draw a rectangle with a given color, width, and height
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        public static void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            Rectangle rectangle = new Rectangle(x, y, width, height);
            spriteBatch.Draw(PixelTexture, rectangle, color);
        }

        /// <summary>
        /// Draw a rectangle given vectors
        /// </summary>
        /// <param name="position"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        public static void DrawRectangle(Vector2 position, int width, int height, Color color)
        {
            DrawRectangle((int)position.X, (int)position.Y, width, height, color);
        }

        /// <summary>
        /// Draws a sprite given a texture name
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