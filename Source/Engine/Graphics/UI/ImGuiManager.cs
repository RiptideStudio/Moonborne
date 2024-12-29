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
using MonoGame.Extended.Timers;
using Moonborne.Graphics.Camera;

namespace Moonborne.Engine.UI
{
    public static class ImGuiManager
    {
        private static MGame game;
        public static ImGUIRenderer imGuiRenderer;

        public static RenderTarget2D tilePreviewRenderTarget;
        public static GraphicsDevice graphicsDevice;

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
            graphicsDevice = device;
            ImGuiIOPtr io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable; // Enable docking

            tilePreviewRenderTarget = new RenderTarget2D(
            graphicsDevice,
            1920, // Width of the preview area
            1080, // Height of the preview area
            false,
            SurfaceFormat.Color,
            DepthFormat.None
            );
        }

        /// <summary>
        /// Start a new ImGui frame
        /// </summary>
        public static void BeginFrame(GameTime gameTime)
        {
            imGuiRenderer.BeginLayout(gameTime);

            ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(mainViewport.Pos);
            ImGui.SetNextWindowSize(mainViewport.Size);
            ImGui.SetNextWindowViewport(mainViewport.ID);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, System.Numerics.Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);

            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoTitleBar |
                                           ImGuiWindowFlags.NoCollapse |
                                           ImGuiWindowFlags.NoResize |
                                           ImGuiWindowFlags.NoMove |
                                           ImGuiWindowFlags.NoBringToFrontOnFocus |
                                           ImGuiWindowFlags.NoBackground |
                                           ImGuiWindowFlags.NoNavFocus;

            ImGui.Begin("DockSpace", windowFlags);
            ImGui.PopStyleVar(2);
            ImGui.DockSpace(ImGui.GetID("DockSpace"), System.Numerics.Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);
            ImGui.End();
        }

        /// <summary>
        /// Send input to ImGui
        /// </summary>
        public static void UpdateInput()
        {
            var io = ImGui.GetIO();

            var mouseState = Mouse.GetState();
            io.MousePos = new System.Numerics.Vector2(mouseState.X, mouseState.Y);
            io.MouseDown[0] = mouseState.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouseState.RightButton == ButtonState.Pressed;

            var keyboardState = Keyboard.GetState();
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                io.KeysDown[(int)key] = keyboardState.IsKeyDown(key);
            }

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
