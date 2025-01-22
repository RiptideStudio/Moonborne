using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Graphics;
using Moonborne.Graphics.Camera;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Objects;
using Moonborne.Input;
using System;
using System.Runtime.Serialization;
using Moonborne.Game.Room;
using System.Collections.Generic;
using Moonborne.Engine.UI;
using Moonborne.UI.Dialogue;
using Moonborne.Graphics.Window;
using Moonborne.Game.Projectiles;
using Moonborne.Game.Inventory;
using MonoGame.Extended.Tiled;
using ImGuiNET;
using Moonborne.Engine.Audio;
using Moonborne.Engine;
using System.IO;

namespace Moonborne
{
    public class MGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        public Player player;
        public static float DeltaTime;

        public MGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            if (GraphicsDevice == null)
            {
                throw new Exception("GraphicsDevice is null. Cannot initialize ImGuiManager.");
            }
            // IsMouseVisible = false;
            GameManager.Initialize(this);
            SpriteManager.Initialize(Content,GraphicsDevice);
            GraphicsManager.Initialize(Content, GraphicsDevice, _graphics, this);
            Camera.Initialize();
            ImGuiManager.Initialize(this, GraphicsDevice);
            WindowManager.Initialize(_graphics,this);
            AudioManager.Initialize();
            DialogueManager.LoadDialogue();
            ConsoleEditor.Initialize();
            Window.TextInput += OnTextInput;
            base.Initialize();
        }

        private void OnTextInput(object sender, TextInputEventArgs e)
        {
            // Log the character and key
            ImGui.GetIO().AddInputCharacter(e.Character);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteManager.LoadAllTextures();
            SpriteManager.spriteBatch = spriteBatch;
            RoomManager.LoadRooms(GraphicsDevice, this);
            InitializeLater();
        }

        protected void InitializeLater()
        {
            InventoryManager.Initialize();
            DialogueManager.InitializeLater();
            RoomEditor.Initialize();
            SettingsManager.Load();

            LayerManager.Initialize();
            LayerManager.UpdateFrame(0);

            Window.Title = "Moonborne Engine";
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            DeltaTime = dt;

            ImGuiManager.UpdateInput();
            InputManager.Update(dt);
            WindowManager.Update(dt);
            Camera.Update();
            DialogueManager.Update(dt);
            InventoryManager.Update(dt);
            AudioManager.Update(dt);

            // Update everything
            LayerManager.Update(dt);

            base.Update(gameTime);

            // Save a snapshoft after releasing mouse (this actually works quite well)
            if (InputManager.MouseLeftReleased())
            {
                RoomManager.SaveSnapshot();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            // Start drawing
            GraphicsDevice.Clear(Color.Black);
            ImGuiManager.BeginFrame(gameTime);

            // Render everything in all layers
            LayerManager.Draw(spriteBatch);

            // End drawing
            ImGuiManager.EndFrame(gameTime);

            // Draw the mouse over ImGui
            Rectangle mouseRect = InputManager.MouseHitbox;
            mouseRect.Width = 8;
            mouseRect.Height = 8;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, WindowManager.Transform);
            // spriteBatch.Draw(SpriteManager.GetTexture("Cursor"), mouseRect, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Called when game is quit
        /// </summary>
        protected override void EndRun()
        {
            base.EndRun();

            // Delete all temporary files on close
            string directory = Directory.GetCurrentDirectory() + "/Content/Temp";
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory,true);
            }
        }
    }
}
