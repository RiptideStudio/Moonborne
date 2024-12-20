/*
 * Author: Callen Betts (2024)
 * Description: Used as a boilerplate for commonly used functions
 */

using System;

namespace Moonborne.Game.Objects
{
    public static class ObjectLibrary
    {
        /// <summary>
        /// Creates a new object given a type; this is generalized and probably won't be used much
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObject<T>() where T : GameObject, new()
        {
            return new T();
        }
    }
}