﻿/*
 * Author: Callen Betts (2024)
 * Description: Used to manage lists of objects during runtime
 */

using Microsoft.Xna.Framework.Graphics;
using Moonborne.Graphics;
using System;
using System.Collections.Generic;

namespace Moonborne.Game.Objects
{
    public static class GameObjectManager
    {
        static List<GameObject> WaitingList = new List<GameObject>(); // List of all of our objects
        static List<GameObject> Objects = new List<GameObject>(); // List of all of our objects
        static List<GameObject> DestroyedObjects = new List<GameObject>(); // List of all of our objects

        /// <summary>
        /// Add a gameobject to the list to update
        /// </summary>
        /// <param name="obj"></param>
        public static void Add(GameObject obj)
        {
            WaitingList.Add(obj);
        }

        /// <summary>
        /// Update our list of game objects
        /// </summary>
        /// <param name="dt"></param>
        public static void Update(float dt)
        {
            // Update all objects
            foreach (GameObject obj in Objects)
            {
                obj.Update(dt);

                // Defer destruction until frame is over
                if (obj.IsDestroyed)
                {
                    DestroyedObjects.Add(obj);
                }
            }

            // Destroy objects after being deferred
            foreach (GameObject obj in DestroyedObjects)
            {
                // Defer destruction until frame is over
                if (obj.IsDestroyed)
                {
                    Objects.Remove(obj);
                }
            }

            // Add objects in the waiting queue
            foreach (GameObject obj in WaitingList)
            {
                Objects.Add(obj);
            }

            WaitingList.Clear();
        }

        /// <summary>
        /// Draw all objects
        /// </summary>
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (GameObject obj in Objects)
            {
                obj.Draw(spriteBatch);
            }
        }
        
        /// <summary>
        /// Execute all draw UI events
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawUI(SpriteBatch spriteBatch)
        {
            foreach (GameObject obj in Objects)
            {
                obj.DrawUI(spriteBatch);
            }
        }
    }
}