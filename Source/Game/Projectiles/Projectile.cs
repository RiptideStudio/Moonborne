/*
 * Author: Callen Betts (2024)
 * Description: Defines a projectile class with many helpful functions and properties
 */

using System;
using Moonborne.Game.Objects;
using Microsoft.Xna.Framework;

namespace Moonborne.Game.Projectiles
{
    public abstract class Projectile : Actor
    {
        public bool CanCollide { get; set; } = true;
        public int LifeTime { get; set; } = 2; // Lifetime in seconds
        public float ElapsedTime { get; set; } = 0;
        public int Penetration = 1;
        public int PenetrationCount = 0;

        /// <summary>
        /// Called when a projectile hits a tile
        /// </summary>
        public virtual void OnTileCollide()
        {
            if (CanCollide)
            {
                Destroy();
            }
        }

        /// <summary>
        /// Hurt the other object
        /// </summary>
        /// <param name="other"></param>
        public override void OnCollisionStart(GameObject other)
        {
            base.OnCollisionStart(other);

            // Damage the other object
            if (other is Actor)
            {
                Actor actor = (Actor)other;

                // Do not hurt friendly actors
                if (actor.Friendly)
                    return;

                // Hurt the other object
                actor.Hurt(Damage);

                // Keep going if we have a large penetration value
                PenetrationCount++;
                if (PenetrationCount >= Penetration)
                {
                    Kill();
                }
            }
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
            Physics.Velocity = launchDirection * speed;
            Transform.Rotation = direction;
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