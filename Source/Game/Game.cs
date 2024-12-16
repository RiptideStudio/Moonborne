using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Graphics;
using Moonborne.Game.Gameplay;
using Moonborne.Input;
using System;

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
            SpriteManager.Initialize(Content,GraphicsDevice);
            GraphicsManager.Initialize(Content, GraphicsDevice, _graphics, this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteManager.LoadAllTextures();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player = new Player();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            InputManager.Update(dt);
            GraphicsManager.Update(dt);

            player.Update(dt);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            player.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
