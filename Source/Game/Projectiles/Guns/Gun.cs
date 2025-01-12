/*
 * Author: Callen Betts (2024)
 * Description: Bullet class inheriting from projectile
 */

using Microsoft.Xna.Framework.Graphics;
using Moonborne.Game.Objects;
using Moonborne.Graphics;
using Microsoft.Xna.Framework;
using System;
using Moonborne.Input;

namespace Moonborne.Game.Projectiles
{
    public class Gun : Actor
    {
        public GameObject Parent { get; set; } // Parent this gun belongs to
        public Vector2 Target { get; set; } = Vector2.One; // Our target look position (usually our mouse)
        public int ShootDelay { get; set; } = 20; // Delay between each possible shot (cooldown)
        public float ShootSpeed { get; set; } = 10f; // Speed of shot
        private float TimeElapsed { get; set; } = 0f; // Used for cooldowns
        private bool CanShoot { get; set; } = true; // Check whether or not we can shoot
        public int Level = 1; // How much this gun has been upgraded

        /// <summary>
        /// Constructor for making a new gun
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="shootSpeed"></param>
        /// <param name="shootDelay"></param>
        public Gun (GameObject parent, float shootSpeed = 10f, int shootDelay = 5) : base()
        {
            SpriteIndex = SpriteManager.GetSprite("None");
            Parent = parent;
            ShootSpeed = shootSpeed;
            ShootDelay = shootDelay;
            Collideable = false;
            Visible = false;
        }

        /// <summary>
        /// Override of the base create event
        /// </summary>
        public override void Create()
        {
            base.Create();
            Speed = 1000;
        }

        /// <summary>
        /// Upgrade this gun
        /// </summary>
        public virtual void Upgrade()
        {
            Level++;
            Damage++;
        }

        /// <summary>
        /// Shoot a projectile from the gun
        /// </summary>
        public virtual void Shoot()
        {
            if (!CanShoot)
            {
                return;
            }

            Bullet bullet = ObjectLibrary.CreateObject<Bullet>(Position, "Projectiles");
            bullet.Damage = Damage;
            bullet.Launch(InputManager.MouseDirection(Position), ShootSpeed);
            CanShoot = false;
        }

        /// <summary>
        /// Override base update and face our target
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            base.Update(dt);
            FaceTarget();

            // Handle cooldowns
            if (!CanShoot)
            {
                TimeElapsed += dt;

                if (TimeElapsed >= ShootDelay*dt)
                {
                    CanShoot = true;
                    TimeElapsed = 0;
                }
            }
        }

        /// <summary>
        /// Face the target we are shooting at
        /// </summary>
        public void FaceTarget()
        {
            Target = InputManager.MouseWorldCoords();
            Vector2 direction = Target - Position;
            Vector2 newOffset = direction;
            newOffset.Normalize();
            newOffset *= 10;
            DrawOffset = newOffset;
            Rotation = MathF.Atan2(direction.Y, direction.X);
            Position = Parent.Position;
        }

        /// <summary>
        /// Set the owner or parent this gun belongs to
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(GameObject parent)
        {
            Parent = parent;
        }
    }
}