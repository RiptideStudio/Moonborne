/*
 * Author: Callen Betts (2024)
 * Description: Used as a boilerplate for commonly used functions
 */

using Microsoft.Xna.Framework;
using Moonborne.Game.Room;

namespace Moonborne.Game.Objects
{
    public static class ObjectLibrary
    {
        /// <summary>
        /// Creates a new object given a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObject<T>(Vector2 position, string layer = "Object") where T : GameObject, new()
        {
            T gameObject = new T();
            gameObject.Position = position;
            gameObject.StartPosition = position;
            LayerManager.AddInstance(gameObject, layer);

            return gameObject;
        }
    }
}