/*
 * Author: Callen Betts (2024)
 * Description: Defines a projectile class with many helpful functions and properties
 */

using System;
using System.Numerics;
using Moonborne.Game.Objects;

namespace Moonborne.Game.Projectiles
{
    public abstract class Projectile : GameObject
    {
        public bool CanCollide { get; set; } = true;

        /// <summary>
        /// Called when a projectile hits a tile
        /// </summary>
        public virtual void OnTileCollide()
        {
            Destroy();
        }

        /// <summary>
        /// Called when a projectile collides with an object that is marked as collideable
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual void OnEnemyCollide(GameObject other)
        {

        }

        /// <summary>
        /// Launches a projectile in a given direction with a given speed
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        public void Launch(float direction, float speed)
        {
            // Convert the angle from degrees into a direction vector
            float xSpeed = MathF.Cos(direction);
            float ySpeed = MathF.Sin(direction);

            Vector2 launchDirection = new Vector2(xSpeed, ySpeed);

            // Apply velocity impulse
            Velocity = launchDirection * speed;
        }
    }
}