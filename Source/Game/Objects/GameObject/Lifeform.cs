using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Inventory;
using Moonborne.Graphics;
using Moonborne.Input;
using Moonborne.Utils.Math;
using System;
using System.Collections.Generic;

namespace Moonborne.Game.Objects
{

    /// <summary>
    /// Extension of game object class. Has better interaction capabilities and more properties
    /// </summary>
    public class Lifeform : Actor
    {

        /// <summary>
        /// Extend the update method
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            base.Update(dt);

            Sprite sprite = Sprites[(int)State, (int)Direction];

            if (sprite != null)
            {
                SpriteIndex = Sprites[(int)State, (int)Direction];
            }

            // Change direction based on velocity
            if (Velocity.Y > 0)
                Direction = Direction.Down;
            if (Velocity.Y < 0)
                Direction = Direction.Up;
            if (Velocity.X > 0)
                Direction = Direction.Right;
            if (Velocity.X < 0)
                Direction = Direction.Left;
        }
    }
}