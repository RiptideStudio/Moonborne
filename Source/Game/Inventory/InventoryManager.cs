/*
 * Author: Callen Betts (2024)
 * Description: Used to manage a small amount of key items collected
 */

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Graphics;
using Moonborne.Input;

namespace Moonborne.Game.Inventory
{
    public static class InventoryManager
    {
        public static List<Item> Items = new List<Item>();
        public static int Columns { get; set; } = 3;
        public static bool Open { get; set; } = false;
        public static Vector2 Position = new Vector2(32, 32);

        /// <summary>
        /// Initialize the inventory with game devices
        /// </summary>
        public static void Initialize()
        {
            AddItem(new LunarCore());
        }

        /// <summary>
        /// Toggle inventory on or off
        /// </summary>
        public static void Toggle()
        {
            Open = !Open;
        }

        /// <summary>
        /// Draw the inventory UI
        /// </summary>
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (!Open)
            {
                return;
            }

            // Iterate over each item and draw it
            for (int i = 0; i < Items.Count; ++i)
            {
                Item item = Items[i];

                Vector2 itemPosition = Position + new Vector2((i % Columns) *32, (i / Columns) * 32);
                item.Position = itemPosition;
                item.Draw(spriteBatch);
            }
        }
         
        /// <summary>
        /// Update the inventory per frame (deprecated)
        /// </summary>
        public static void Update()
        {
            if (InputManager.KeyTriggered(Keys.Tab))
            {
                Toggle();
            }
        }

        /// <summary>
        /// Add an item to the inventory
        /// </summary>
        /// <param name="item"></param>
        public static void AddItem(Item item)
        {
            Items.Add(item);
        }
    }
}