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
    public class Player : Lifeform
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
            MaxSpeed = 75;
            Instance = this;

            // Set our directional sprites
            SetSprite("PlayerIdleLeft", 16, 16, State.Idle, Direction.Left);
            SetSprite("PlayerIdleRight", 16, 16, State.Idle, Direction.Right);
            SetSprite("PlayerIdleDown", 16, 16, State.Idle, Direction.Down);
            SetSprite("PlayerIdleUp", 16, 16, State.Idle, Direction.Up);

            SetSprite("PlayerWalkLeft", 16, 16, State.Move, Direction.Left);
            SetSprite("PlayerWalkRight", 16, 16, State.Move, Direction.Right);
            SetSprite("PlayerWalkDown", 16, 16, State.Move, Direction.Down);
            SetSprite("PlayerWalkUp", 16, 16, State.Move, Direction.Up);

            SpriteIndex = GetSprites(State.Idle, Direction.Down);

            Camera.SetPosition(Position);
            Gun = new Gun(this,250f,10);
            LayerManager.AddInstance(Gun, "Object");
            Hitbox.Width = 8;
            Hitbox.Height = 8;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // DrawShadow(Position.X+1,Position.Y+6,6,2);
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

            bool moving = false;

            if (InputManager.KeyDown(Keys.W))
            {
                Velocity.Y = -Speed;
                Direction = Direction.Up;
                moving = true;
            }
            if (InputManager.KeyDown(Keys.S))
            {
                Velocity.Y = Speed;
                Direction = Direction.Down;
                moving = true;
            }
            if (InputManager.KeyDown(Keys.A))
            {
                Velocity.X = -Speed;
                moving = true;
                Direction = Direction.Left;
            }
            if (InputManager.KeyDown(Keys.D))
            {
                Velocity.X = Speed;
                moving = true;
                Direction = Direction.Right;
            }

            if (moving)
            {
                State = State.Move;
            }
            else
            {
                State = State.Idle;
            }
        }
    }
}