using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Graphics;
using Moonborne.Game.Objects;
using Moonborne.Input;
using Moonborne.Graphics.Camera;
using Moonborne.Game.Projectiles;
using Moonborne.Game.Inventory;
using Moonborne.Game.Room;
using System;

namespace Moonborne.Game.Gameplay
{
    public class Player : Actor
    {
        public Gun Gun { get; set; }
        public static Player Instance { get; private set; }
        public string DisplayName { get; set; } = "Player";
        public override void Create()
        {
            base.Create();

            AnimationSpeed = 10;
            Scale = new Vector2(1, 1);
            Position.X = 400;
            Position.Y = 400;
            Collideable = true;
            Speed = 100;
            MaxSpeed = 100;
            Instance = this;

            // Set our sprites
            IdleSprites[Direction.Left] = SpriteManager.GetSprite("PlayerIdleLeft");
            IdleSprites[Direction.Right] = SpriteManager.GetSprite("PlayerIdleRight");
            IdleSprites[Direction.Up] = SpriteManager.GetSprite("PlayerIdleUp");
            IdleSprites[Direction.Down] = SpriteManager.GetSprite("PlayerIdleDown");
            SpriteIndex = IdleSprites[Direction.Down];

            Camera.SetPosition(Position);
            Gun = new Gun(this,250f,10);
            LayerManager.AddInstance(Gun, "Object");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawShadow(Position.X+1,Position.Y+6,6,2);
            base.Draw(spriteBatch);
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            // Shoot a test projectile
            if (InputManager.KeyDown(Keys.C))
            {
                Gun.Shoot();
            }

            if (InputManager.KeyDown(Keys.W))
            {
                Velocity.Y = -Speed;
                Direction = Direction.Up;
            }
            if (InputManager.KeyDown(Keys.A))
            {
                Velocity.X = -Speed;
                Direction = Direction.Left;
            }
            if (InputManager.KeyDown(Keys.S))
            {
                Velocity.Y = Speed;
                Direction = Direction.Down;
            }
            if (InputManager.KeyDown(Keys.D))
            {
                Velocity.X = Speed;
                Direction = Direction.Right;
            }
        }
    }
}