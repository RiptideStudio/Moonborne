/*
 * Author: Callen Betts (2024)
 * Description: Used to manage a small amount of key items collected
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Engine.UI;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Objects;
using Moonborne.Game.Room;
using Moonborne.Graphics;
using Moonborne.Input;

namespace Moonborne.Game.Inventory
{
    public static class InventoryManager
    {
        public static List<Slot> Slots = new List<Slot>();
        public static int Cores = 10;
        public static int Columns { get; set; } = 3;
        public static bool Open { get; set; } = false;
        public static Vector2 Position = new Vector2(-132, 16);
        public static List<GameAction> Actions = new List<GameAction>();
        public static GameObject InventoryObject;

        /// <summary>
        /// Initialize the inventory with game devices
        /// </summary>
        public static void Initialize()
        {
            AddItem(new LunarCore());
            InventoryObject = ObjectLibrary.CreateObject("EmptyObject", Position, "Managers");
        }

        /// <summary>
        /// Toggle inventory on or off
        /// </summary>
        public static void Toggle()
        {
            Open = !Open;

            if (Open)
            {
                InventoryObject.AddAction(new MoveAction(new Vector2(32, 16)),false,true);
            }
            else
            {
                InventoryObject.AddAction(new MoveAction(new Vector2(-132, 16)),false,true);
            }
        }

        /// <summary>
        /// Draw the inventory UI
        /// </summary>
        public static void Draw(SpriteBatch spriteBatch)
        {
            SpriteManager.SetDrawAlpha(0.75f);
            SpriteManager.DrawRectangle(Position, 128,240, Color.Black);
            SpriteManager.SetDrawAlpha(1);

            // Iterate over each item and draw it
            for (int i = 0; i < Slots.Count; ++i)
            {
                Slot slot = Slots[i];

                Vector2 slotPosition = Position + new Vector2((i % Columns) *32, (i / Columns) * 32);
                slot.Position = slotPosition;
                slot.Draw(spriteBatch);
            }
        }
         
        /// <summary>
        /// Update the inventory per frame (deprecated)
        /// </summary>
        public static void Update(float dt)
        {
            Position = InventoryObject.Transform.Position;

            if (InputManager.KeyTriggered(Keys.Tab))
            {
                Toggle();
            }
        }

        /// <summary>
        /// Remove an item from the inventory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public static void RemoveItem(Type item, int amount = 1)
        {
            Slot targetSlot = null;

            foreach (Slot slot in Slots)
            {
                if (slot.Item.ToString() == item.ToString())
                {
                    // Add to the slot
                    slot.Amount -= amount;
                    if (slot.Amount <= 0)
                    {
                        targetSlot = slot;
                    }
                }
            }

            // Delete slot if it is empty
            if (targetSlot != null)
            {
                Slots.Remove(targetSlot);
            }
        }

        /// <summary>
        /// Check if we have an amount of an item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static bool HasItem(Type item, int amount)
        {
            foreach (Slot slot in Slots)
            {
                if (slot.Item.ToString() == item.ToString())
                {
                    // Add to the slot
                    if (slot.Amount >= amount)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Add an item to the inventory
        /// </summary>
        /// <param name="item"></param>
        public static void AddItem(Item item, int amount = 1)
        {
            bool found = false;

            // Check if we can add this item to a slot
            foreach (Slot slot in Slots)
            {
                if (slot.Item.Name == item.Name)
                {
                    // Add to the slot
                    slot.Amount += amount;
                    found = true;
                }
            }

            // Create a new slot 
            if (!found)
            {
                Slots.Add(new Slot(item, amount));
            }
        }
    }
}