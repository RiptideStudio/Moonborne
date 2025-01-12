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
using System.Collections;

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
        public static SpriteBatch TargetSpriteBatch;
        private static Dictionary<Texture2D, IntPtr> textureCache = new Dictionary<Texture2D, IntPtr>();
        public static SpriteFont GameFont;
        public static Texture2D PixelTexture;
        public static byte DrawAlpha = 1;
        public static bool DrawCentered = false;

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
            GameFont = content.Load<SpriteFont>("Fonts/PixelFont");
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
                        Sprite sprite = new Sprite(texture);

                        // Assuming same dimensions for width and height by default
                        sprite.MaxFrames = 1;
                        sprite.FrameWidth = texture.Width;
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
        public static void DrawSprite(string spriteName, int frame, Vector2 position, Vector2 scale, float rotation, Color color)
        {
            Sprite sprite = GetSprite(spriteName);

            // Draw a sprite given parameters, and use UI positioning if defined as such
            if (sprite != null)
            {
                sprite.Draw(spriteBatch, frame, position, scale, rotation, color);
            }
        }

        /// Draw text with optional scale, rotation, and UI overlay handling.
        /// </summary>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The position to draw the text.</param>
        /// <param name="scale">The scaling factor for the text.</param>
        /// <param name="rotation">The rotation of the text in radians.</param>
        /// <param name="ui">If true, the text is drawn without transformations (for UI elements).</param>
        public static void DrawText(string text, Vector2 position, Vector2 scale, float rotation, Color color, int maxWidth = 9999)
        {
            // Check for invalid input
            if (string.IsNullOrEmpty(text))
                return;

            // Word-wrapping logic
            maxWidth -= (int)(scale.X*GameFont.MeasureString(" ").X);
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
            color *= (float)DrawAlpha/255;

            Vector2 origin = Vector2.Zero;

            if (DrawCentered)
            {
                Vector2 stringSize = GameFont.MeasureString(lines[0]);
                origin.X = stringSize.X / 2;
                origin.Y = stringSize.Y / 2;
            }

            foreach (var line in lines)
            {
                spriteBatch.DrawString(GameFont, line, linePosition, color, rotation, origin, scale, SpriteEffects.None, 0);
                linePosition.Y += GameFont.LineSpacing * (scale.Y/1.25f);
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
        public static void DrawRectangle(float x, float y, int width, int height, Color color)
        {
            color.A = (byte)Math.Clamp((int)(color.A * (DrawAlpha / 255f)), 0, 255);

            Vector2 origin = Vector2.Zero;

            if (DrawCentered)
            {
                origin.X = width/ 2;
                origin.Y = height / 2;
            }

            Rectangle rectangle = new Rectangle((int)x, (int)y, width, height);
            spriteBatch.Draw(PixelTexture, rectangle, rectangle, color, 0, origin,SpriteEffects.None, 0);
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
            DrawRectangle(position.X, position.Y, width, height, color);
        }               
        
        /// <summary>
        /// Draw rectangle that uses floats
        /// </summary>
        /// <param name="position"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        public static void DrawRectangle(Vector2 position, float width, float height, Color color)
        {
            DrawRectangle(position.X, position.Y, (int)width, (int)height, color);
        }       
        
        /// <summary>
        /// Draw a rectangle given a rectangle struct
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="color"></param>
        public static void DrawRectangle(Rectangle rectangle, Color color)
        {
            DrawRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color);
        }

        /// <summary>
        /// Draw an ellipse given a radius
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawEllipse(float x, float y, float xRadius, float yRadius, Color color)
        {
            // Assume EllipseTexture is a preloaded Texture2D of a white ellipse
            Vector2 position = new Vector2(x - xRadius*2, y - yRadius); // Top-left corner
            Vector2 size = new Vector2(xRadius * 2, yRadius * 2); // Scale to match radii
            color.A = DrawAlpha;
            Vector2 origin = new Vector2(xRadius, yRadius);

            Texture2D EllipseTexture = GetTexture("Ellipse");
            spriteBatch.Draw(EllipseTexture, position, null, color, 0f, origin, size / new Vector2(EllipseTexture.Width, EllipseTexture.Height), SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Draw a line between two points
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="thickness"></param>
        /// <param name="color"></param>
        public static void DrawLine(Vector2 start, Vector2 end, int thickness, Color color)
        {
            float length = Vector2.Distance(start, end);

            float angle = MathF.Atan2(end.Y - start.Y, end.X - start.X);
            spriteBatch.Draw(
            PixelTexture,
                start,
                null,
                color,
                angle,
                Vector2.Zero,
                new Vector2(length, thickness),
                SpriteEffects.None,
                0
            );
        }

        /// <summary>
        /// Set the target render alpha. Use ResetDraw() after done!
        /// </summary>
        /// <param name="alpha"></param>
        public static void SetDrawAlpha(float alpha)
        {
            alpha = Math.Clamp(alpha, 0.0f, 1.0f);
            DrawAlpha = (byte)(alpha*255);
        }

        /// <summary>
        /// Set the draw alignment
        /// </summary>
        /// <param name="centered"></param>
        public static void DrawSetAlignment(bool centered)
        {
            DrawCentered = centered;
        }

        /// <summary>
        /// Reset the draw properties to default
        /// </summary>
        public static void ResetDraw()
        {
            DrawAlpha = 255;
            DrawCentered = false;
        }

        /// <summary>
        /// Retrieve a texture given a name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(string name)
        {
            // Return an empty texture if not found
            if (name == null || !textures.ContainsKey(name))
            {
                return textures["None"];
            }

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

        /// <summary>
        /// Get a sprite and set its values for animation, returning that newly modified sprite
        /// </summary>
        /// <param name="name"></param>
        /// <param name="frameWidth"></param>
        /// <param name="frameHeight"></param>
        /// <returns></returns>
        public static Sprite AssignSprite(string name, int frameWidth, int frameHeight)
        {
            Sprite sprite = GetSprite(name);

            if (sprite != null)
            {
                sprite.FrameHeight = frameHeight;
                sprite.FrameWidth = frameWidth;
                sprite.MaxFrames = sprite.Texture.Width / frameHeight;
            }

            return sprite;
        }
    }
}