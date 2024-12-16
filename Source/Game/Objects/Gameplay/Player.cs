using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Graphics;
using Moonborne.Game.Components;
using System.Collections.Generic;
using Moonborne.Game.Objects;
using Moonborne.Input;
using System;

namespace Moonborne.Game.Gameplay
{
    public class Player : GameObject
    {
        private Animation IdleAnimation;
        
        public override void Create()
        {
            sprite.SetSpritesheet("JungleTileset", 160, 160, 1);
            AnimationSpeed = 1;
            Scale = new Vector2(4, 4);
            Speed = 150;
        }

        public override void Update(float dt)
        {
            if (InputManager.KeyDown(Keys.W))
            {
                Velocity.Y = -Speed;
            }
            if (InputManager.KeyDown(Keys.A))
            {
                sprite.CustomSpriteEffect = SpriteEffects.FlipHorizontally;
                Velocity.X = -Speed;
            }
            if (InputManager.KeyDown(Keys.S))
            {
                Velocity.Y = Speed;


            }
            if (InputManager.KeyDown(Keys.D))
            {
                sprite.CustomSpriteEffect = SpriteEffects.None;
                Velocity.X = Speed;
            }
            base.Update(dt);
        }
    }
}