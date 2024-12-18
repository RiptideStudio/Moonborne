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
        /// End ImGui frame
        /// </summary>
        public static void EndFrame(GameTime gameTime)
        {
            imGuiRenderer.EndLayout();
        }
    }
}
