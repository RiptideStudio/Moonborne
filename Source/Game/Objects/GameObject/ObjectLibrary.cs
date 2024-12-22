/*
 * Author: Callen Betts (2024)
 * Description: Used as a boilerplate for commonly used functions
 */

using Microsoft.Xna.Framework;

namespace Moonborne.Game.Objects
{
    public static class ObjectLibrary
    {
        /// <summary>
        /// Creates a new object given a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObject<T>(Vector2 position) where T : GameObject, new()
        {
            T t = new T();
            t.Position = position;
            t.StartPosition = position;
            return t;
        }
    }
}