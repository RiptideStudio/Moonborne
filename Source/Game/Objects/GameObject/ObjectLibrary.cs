/*
 * Author: Callen Betts (2024)
 * Description: Used as a boilerplate for commonly used functions
 */

using Microsoft.Xna.Framework;
using Moonborne.Game.Room;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using MonoGame.Extended.Collisions.Layers;

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


        private static readonly Dictionary<string, Type> gameObjectTypes;

        static ObjectLibrary()
        {
            // Discover all subclasses of GameObject
            gameObjectTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(GameObject))
                               && !type.IsAbstract
                               && HasParameterlessConstructor(type)) // Only include types with default constructors
                .ToDictionary(type => type.Name, type => type);
        }

        private static bool HasParameterlessConstructor(Type type)
        {
            // Check if the type has a parameterless constructor
            return type.GetConstructors()
                       .Any(ctor => ctor.GetParameters().Length == 0);
        }

        public static List<string> GetAllGameObjectNames()
        {
            return gameObjectTypes.Keys.ToList();
        }

        public static GameObject CreateObject(string name, Vector2 position, string layerName = "Object")
        {
            if (!gameObjectTypes.TryGetValue(name, out var type))
            {
                throw new Exception($"GameObject type '{name}' not found.");
            }

            // Create the object
            var obj = (GameObject)Activator.CreateInstance(type);
            obj.Position = position;
            obj.StartPosition = position;
            obj.NeedsLayerSort = true;
            obj.CreateLater(); // post-intialize create

            LayerManager.AddInstance(obj, layerName);

            return obj;
        }
    }
}