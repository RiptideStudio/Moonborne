/*
 * Author: Callen Betts (2024)
 * Description: Moon core currency item
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Graphics;

namespace Moonborne.Game.Inventory
{
    public class Slot
    {
        public Item Item;
        public int Amount;
        public Vector2 Position;

        public Slot(Item item, int amount)
        {
            Item = item;
            Amount = amount;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Item != null)
            {
                Item.Draw(spriteBatch);
                SpriteManager.DrawText($"{Item.Name}: x{Amount}", Position, Vector2.One, 0, Color.White);
            }
        }
    }
}