using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Input;
using System;

namespace Moonborne.Graphics
{
    public static class GraphicsManager
    {
        public static ContentManager content;
        public static GraphicsDevice graphicsDevice;
        public static GraphicsDeviceManager graphics;
        public static MGame game;

        public static bool StartInFullscreen = false;
        public static bool IsFullscreen = false;
        public static Point WindowSize = new Point(1280,720);
        public static float TargetFrameRate = 1.0f / 60.0f;

        /// <summary>
        /// Set up our graphics manager, and set our base window height
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="device"></param>
        /// <param name="graphics_"></param>
        public static void Initialize(ContentManager contentManager, GraphicsDevice device, GraphicsDeviceManager graphics_, MGame game_)
        {
            content = contentManager;
            graphicsDevice = device;
            graphics = graphics_;
            game = game_;

            SetWindowSize(WindowSize);
            SetTargetFramerate(TargetFrameRate);

            if (StartInFullscreen)
            {
                ToggleFullsceen();
            }
        }

        /// <summary>
        /// Update our target framerate: this should always be 60 fps
        /// </summary>
        /// <param name="targetFrameRate"></param>
        public static void SetTargetFramerate(float targetFrameRate)
        {
            game.TargetElapsedTime = TimeSpan.FromSeconds(targetFrameRate);
        }

        /// <summary>
        /// Toggles fullscreen
        /// </summary>
        public static void ToggleFullsceen()
        {
            graphics.ToggleFullScreen();
            IsFullscreen = !IsFullscreen;

            if (IsFullscreen)
            {
                // Do nothing 
            }
            else
            {
                // Set our window size
                SetWindowSize(WindowSize);
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
        /// Set our window size
        /// </summary>
        /// <param name="point"></param>
        public static void SetWindowSize(Point point)
        {
            graphics.PreferredBackBufferWidth = point.X;
            graphics.PreferredBackBufferHeight = point.Y;
            graphics.ApplyChanges();
        }
    }
}