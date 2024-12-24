/*
 * Author: Callen Betts (2024)
 * Description: Defines a projectile class with many helpful functions and properties
 */

using System;
using Moonborne.Game.Objects;
using Microsoft.Xna.Framework;

namespace Moonborne.Game.Projectiles
{
    public abstract class Projectile : GameObject
    {
        public bool CanCollide { get; set; } = true;
        public int LifeTime { get; set; } = 2; // Lifetime in seconds
        public float ElapsedTime { get; set; } = 0;

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
        /// Updates a projectile, calling base update
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            base.Update(dt);

            ElapsedTime += dt;

            if (ElapsedTime >= LifeTime)
            {
                // Kill projectile
                Destroy();
            }
        }

        /// <summary>
        /// Launches a projectile in a given direction with a given speed
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        public void Launch(float direction, float speed)
        {
            // Convert the angle from degrees into a direction vector
            direction *= MathF.PI / 180;
            float xSpeed = MathF.Cos(direction);
            float ySpeed = MathF.Sin(direction);
            Vector2 launchDirection = new Vector2(xSpeed, ySpeed);

            // Apply velocity impulse and change rotation
            Velocity = launchDirection * speed;
            Rotation = direction;
        }

        /// <summary>
        /// Launches a projectile in a given vector direction and speed
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        public void Launch(Vector2 direction, float speed)
        {
            float angle = MathF.Atan2(direction.Y, direction.X)* 180/MathF.PI;
            Launch(angle, speed);
        }
    }
}