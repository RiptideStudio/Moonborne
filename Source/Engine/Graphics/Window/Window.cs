/*
 * Author: Callen Betts (2024)
 * Description: Manages window properties and viewport scaling
 */

using System;
using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Game.Objects;
using Moonborne.Graphics;
using Moonborne.Input;

namespace Moonborne.Graphics.Window
{
    public static class WindowManager
    {
        public static int ViewportWidth { get; private set; } = 320; // Height of the viewport
        public static int ViewportHeight { get; private set; } = 180; // Height of the viewport
        public static int BaseViewportWidth { get; private set; } = 320; // Height of the viewport
        public static int BaseViewportHeight { get; private set; } = 180; // Height of the viewport
        public static int ViewportScale { get; private set; } = 6; // Scale of the viewport 
        public static int PreviousViewportScale { get; private set; } = ViewportScale; // Scale of the viewport 
        public static bool StartInFullscreen { get; private set; } = false; // If we start in fullscreen
        public static Rectangle Viewport { get; private set; } // Defines the dimensions of our viewport
        private static GraphicsDeviceManager Graphics { get; set; } // Graphics device
        private static MGame Game { get; set; } // Graphics device
        public static Matrix Transform; // Transformation matrix for UI
        public static bool IsFullscreen = false; // If we are in fullscreen

        /// <summary>
        /// Initialize window manager with graphics
        /// </summary>
        /// <param name="graphics"></param>
        public static void Initialize(GraphicsDeviceManager graphics, MGame game)
        {
            Graphics = graphics;
            Game = game;

            SetResolutionScale(ViewportScale);

            if (StartInFullscreen)
            {
                ToggleFullsceen();
            }

        }

        /// <summary>
        /// Check for keybinds
        /// </summary>
        /// <param name="dt"></param>
        public static void Update(float dt)
        {
            // Fullscreen toggle
            if (InputManager.KeyTriggered(Keys.F11))
            {
                ToggleFullsceen();
            }
        }

        /// <summary>
        /// Toggles fullscreen
        /// </summary>
        public static void ToggleFullsceen()
        {
            IsFullscreen = !IsFullscreen;
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            if (IsFullscreen)
            {
                // Keep track of the previous viewport scale
                PreviousViewportScale = ViewportScale;
                SetResolutionScale(CalculateMaxScale());

                // Toggle fullscreen mode for black bars
                Graphics.ToggleFullScreen();
            }
            else
            {
                // Reset to what game was like before fullscreen
                SetResolutionScale(PreviousViewportScale);
                Graphics.ToggleFullScreen();
            }
        }

        /// <summary>
        /// Update the window height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void SetWindowSize(int width, int height)
        {
            // Update viewport size
            ViewportWidth = width;
            ViewportHeight = height;

            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Graphics.PreferredBackBufferWidth = ViewportWidth;
            Graphics.PreferredBackBufferHeight = ViewportHeight;

            // Letterbox (center the viewport)
            int viewportX = (screenWidth - ViewportWidth) / 2;
            int viewportY = (screenHeight - ViewportHeight) / 2;
            Viewport = new Rectangle(viewportX, viewportY, ViewportWidth, ViewportHeight);

            // Update the graphics device viewport
            Graphics.GraphicsDevice.Viewport = new Viewport(Viewport);

            // Create a transformation matrix to scale the game world
            Transform = Matrix.CreateScale(ViewportScale);
            Graphics.ApplyChanges();
        }

        /// <summary>
        /// Update the resolution scale of the game. This is in integers and will auto clamp.
        /// </summary>
        /// <param name="scale"></param>
        public static void SetResolutionScale(int scale)
        {
            int maxDisplayScale = CalculateMaxScale();
            ViewportScale = Math.Clamp(scale, 1, maxDisplayScale);
            ViewportWidth = BaseViewportWidth * ViewportScale;
            ViewportHeight = BaseViewportHeight * ViewportScale;

            SetWindowSize(ViewportWidth, ViewportHeight);
        }

        /// <summary>
        /// Calculates the maximum scale that fits within the current screen resolution.
        /// </summary>
        public static int CalculateMaxScale()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            // Calculate the maximum scale that fits within the screen dimensions
            int maxScaleX = screenWidth / BaseViewportWidth;
            int maxScaleY = screenHeight / BaseViewportHeight;

            return Math.Min(maxScaleX, maxScaleY);
        }
    }
}
