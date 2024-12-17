using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Graphics;
using Moonborne.Game.Components;
using System.Collections.Generic;
using Moonborne.Game.Objects;
using Moonborne.Input;
using System;
using Moonborne.Graphics.Camera;

namespace Moonborne.Game.Gameplay
{
    public class Player : GameObject
    {
        public override void Create()
        {
            SpriteIndex = SpriteManager.GetSprite("HornetIdle");
            AnimationSpeed = 10;
            Scale = new Vector2(1, 1);
            Speed = 100;
            MaxSpeed = 100;
            Camera.SetTarget(this);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            for (int i = 0; i < 64; ++i)
            {
                for (int j = 0; j < 64; j++)
                {
                    SpriteManager.DrawTileFromTileset("JungleTileset", 1, 16, new Vector2(5+i*16, 5+j*16), Vector2.One);
                }
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            Acceleration = Vector2.Zero;

            if (InputManager.KeyDown(Keys.W))
            {
                Velocity.Y = -Speed;
                SpriteIndex = SpriteManager.GetSprite("HornetIdle");
            }
            if (InputManager.KeyDown(Keys.A))
            {
                SpriteIndex.Flip(Sprite.Axis.Horizontal);
                Velocity.X = -Speed;
            }
            if (InputManager.KeyDown(Keys.S))
            {
                Velocity.Y = Speed;
            }
            if (InputManager.KeyDown(Keys.D))
            {
                SpriteIndex.Flip(Sprite.Axis.None);
                Velocity.X = Speed;
            }
        }
    }
}