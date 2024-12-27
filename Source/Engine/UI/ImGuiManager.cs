using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Moonborne.Graphics;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGui;
using Microsoft.Xna.Framework.Input;
using Moonborne.Input;

namespace Moonborne.Engine.UI
{
    public static class ImGuiManager
    {
        private static MGame game;
        public static ImGUIRenderer imGuiRenderer;

        /// <summary>
        /// Setup ImGui
        /// </summary>
        /// <param name="game"></param>
        public static void Initialize(MGame game_, GraphicsDevice device)
        {
            game = game_;
            ImGui.CreateContext();
            imGuiRenderer = new ImGUIRenderer(game);
            imGuiRenderer.RebuildFontAtlas();
        }

        /// <summary>
        /// Start a new ImGui frame
        /// </summary>
        public static void BeginFrame(GameTime gameTime)
        {
            imGuiRenderer.BeginLayout(gameTime);
        }

        /// <summary>
        /// Send input to ImGui
        /// </summary>
        public static void UpdateInput()
        {

        }

        /// <summary>
        /// End ImGui frame
        /// </summary>
        public static void EndFrame(GameTime gameTime)
        {
            imGuiRenderer.EndLayout();
        }

        /// <summary>
        /// Wrapper for drawing a button at any position
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static bool Button(string name, Vector2 position, Vector2 scale)
        {
            ImGui.SetNextWindowPos(position.ToNumerics());
            return ImGui.Button(name, scale.ToNumerics());
        }


        /// <summary>
        /// Begin a new window with no background
        /// </summary>
        /// <param name="background"></param>
        public static void BeginInvisibleWindow(string name = "InvisibleWindow")
        {
            ImGui.Begin("Invisible Window", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground);
        }
    }
}
