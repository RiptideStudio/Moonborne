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

namespace Moonborne
{
    public class MGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        public Player player;

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

            SpriteManager.Initialize(Content,GraphicsDevice);
            GraphicsManager.Initialize(Content, GraphicsDevice, _graphics, this);
            Camera.Initialize();
            ImGuiManager.Initialize(this, GraphicsDevice);
            WindowManager.Initialize(_graphics,this);
            DialogueManager.LoadDialogue();

            base.Initialize();
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
            player = new Player();
            InventoryManager.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            InputManager.Update(dt);
            WindowManager.Update(dt);
            Camera.Update();
            GameObjectManager.Update(dt);
            DialogueManager.Update(dt);
            InventoryManager.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            ImGuiManager.BeginFrame(gameTime);

            // Draw game world objects
            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                transformMatrix: Camera.Transform);

            RoomEditor.DrawGrid(spriteBatch,SpriteManager.GetTexture("JungleTileset"), 16, 10);
            GameObjectManager.Draw(spriteBatch);

            spriteBatch.End();

            // Draw UI objects
            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                transformMatrix: WindowManager.Transform);

            GameObjectManager.DrawUI(spriteBatch);
            DialogueManager.DrawDialogueBox();
            InventoryManager.Draw(spriteBatch);
            RoomEditor.DrawTilesetPreview(spriteBatch, SpriteManager.GetTexture("JungleTileset"), 16, 10, 32, 32);
            RoomEditor.HandleTileSelection(SpriteManager.GetTexture("JungleTileset"), 16, 10, 32, 32);
            spriteBatch.End();

            ImGuiManager.EndFrame(gameTime);

            base.Draw(gameTime);
        }
    }
}
