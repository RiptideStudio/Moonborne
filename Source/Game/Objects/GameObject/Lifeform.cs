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
    public abstract class Lifeform : Actor
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
            if (Physics.Velocity.Y > 0)
                Direction = Direction.Down;
            if (Physics.Velocity.Y < 0)
                Direction = Direction.Up;
            if (Physics.Velocity.X > 0)
                Direction = Direction.Right;
            if (Physics.Velocity.X < 0)
                Direction = Direction.Left;
        }
    }
}