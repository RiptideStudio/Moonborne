/*
 * Author: Callen Betts (2024)
 * Description: Defines base item class
 */

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Objects;
using Moonborne.Graphics;
using Moonborne.Input;
using Moonborne.Utils.Math;

namespace Moonborne.Game.Inventory
{
    public class Item : GameObject
    {
        public float PickupRange { get; set; } = 32f;

        /// <summary>
        /// Extend the create method
        /// </summary>
        public override void Create()
        {
            base.Create();
            SpriteIndex.SetSpritesheet("MoonCore");
        }

        /// <summary>
        /// Draw extension
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Allow us to pick the item up
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            base.Update(dt);

            // Pickup our item if we're close
            if (InputManager.KeyTriggered(Keys.E))
            {
                if (MoonMath.Distance(Transform.Position, Player.Instance.Transform.Position) < PickupRange)
                {
                    OnPickup();
                }
            }
        }

        /// <summary>
        /// Called when an item is picked up
        /// </summary>
        public virtual void OnPickup()
        {
            InventoryManager.AddItem(this,1);
            Destroy();
        }
    }
}