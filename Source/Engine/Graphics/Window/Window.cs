using System;
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
        public static int ViewportWidth { get; private set; } = 640; // Width of the window viewport
        public static int ViewportHeight { get; private set; } = 360; // Height of the viewport
        public static int ViewportScale { get; private set; } = 2; // Scale of the viewport 
        public static bool StartInFullscreen { get; private set; } = false;
        public static bool IsFullscreen { get; private set; } = false;
        private static GraphicsDeviceManager Graphics { get; set; }

        /// <summary>
        /// Initialize window manager with graphics
        /// </summary>
        /// <param name="graphics"></param>
        public static void Initialize(GraphicsDeviceManager graphics)
        {
            Graphics = graphics;

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
            Graphics.ToggleFullScreen();
            IsFullscreen = !IsFullscreen;

            if (IsFullscreen)
            {
                // Do nothing 
            }
            else
            {
                // Set our window size
                SetWindowSize(ViewportWidth, ViewportHeight);
            }
        }

        /// <summary>
        /// Update the window height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void SetWindowSize(int width, int height)
        {
            Graphics.PreferredBackBufferWidth = ViewportWidth;
            Graphics.PreferredBackBufferHeight = ViewportHeight;
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
            ViewportHeight *= ViewportScale;
            ViewportWidth *= ViewportScale;

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
            int maxScaleX = screenWidth / ViewportWidth;
            int maxScaleY = screenHeight / ViewportHeight;

            return Math.Min(maxScaleX, maxScaleY);
        }
    }
}
