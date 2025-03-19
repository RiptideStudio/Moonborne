using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Input;
using Moonborne.Engine.UI;
using System;
using Moonborne.Engine.Graphics.Lighting;

namespace Moonborne.Graphics
{
    public static class GraphicsManager
    {
        public static ContentManager content;
        public static GraphicsDevice graphicsDevice;
        public static GraphicsDeviceManager graphics;
        public static LightManager lightManager;
        public static MGame game;

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
            lightManager = new LightManager(graphicsDevice);

            SetTargetFramerate(TargetFrameRate);
        }

        /// <summary>
        /// Update our target framerate: this should always be 60 fps
        /// </summary>
        /// <param name="targetFrameRate"></param>
        public static void SetTargetFramerate(float targetFrameRate)
        {
            game.TargetElapsedTime = TimeSpan.FromSeconds(targetFrameRate);
        }
    }
}