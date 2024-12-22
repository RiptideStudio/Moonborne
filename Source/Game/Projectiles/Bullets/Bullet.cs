/*
 * Author: Callen Betts (2024)
 * Description: Bullet class inheriting from projectile
 */

using Microsoft.Xna.Framework.Graphics;
using Moonborne.Graphics;

namespace Moonborne.Game.Projectiles
{
    public class Bullet : Projectile
    {
        public override void Create()
        {
            Speed = 15f;
            SpriteIndex = SpriteManager.GetSprite("Bullet");
            LinearFriction = 0;
        }
    }
}