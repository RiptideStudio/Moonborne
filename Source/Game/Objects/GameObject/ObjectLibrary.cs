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
using Moonborne.Game.Gameplay;
using Moonborne.Engine.Components;
using Moonborne.Game.Objects.Prefabs;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace Moonborne.Game.Objects
{
    public static class ObjectLibrary
    {
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

        /// <summary>
        /// Creates a new object given a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObject<T>(Vector2 position, string layer = "Object") where T : GameObject, new()
        {
            T gameObject = new T();
            gameObject.Transform.Position = position;
            gameObject.StartPosition = position;
            gameObject.NeedsLayerSort = true;
            gameObject.CreateLater();

            LayerManager.AddInstance(gameObject, layer);

            return gameObject;
        }

        /// <summary>
        /// Create a game object of any type
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static GameObject CreateObject(string name)
        {
            if (!gameObjectTypes.TryGetValue(name, out var type))
            {
                Console.Write($"GameObject type '{name}' not found.");
                return new EmptyObject();
            }

            // Create the object
            var obj = (GameObject)Activator.CreateInstance(type);

            return obj;
        }

        /// <summary>
        /// Create a game object of any type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static GameObject CreateObject(string name, Vector2 position, string layerName = "Object")
        {
            if (!gameObjectTypes.TryGetValue(name, out var type))
            {
                Console.Write($"GameObject type '{name}' not found.");
                return new EmptyObject();
            }

            // Create the object
            var obj = (GameObject)Activator.CreateInstance(type);
            obj.Transform.Position = position;
            obj.StartPosition = position;
            obj.NeedsLayerSort = true;
            obj.CreateLater();

            LayerManager.AddInstance(obj, layerName);

            return obj;
        }

        /// <summary>
        /// Create a new instance of a prefab
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static GameObject CreatePrefab(string typeName, string displayName, Vector2 position, string layerName = "Object")
        {
            if (!gameObjectTypes.TryGetValue(typeName, out var type))
            {
                Console.Write($"GameObject type '{typeName}' not found.");
                return new EmptyObject();
            }

            string prefabFilePath = @"Content/Data/Prefabs";

            if (!File.Exists(prefabFilePath+"/"+ displayName+".json"))
            {
                return new EmptyObject();
            }

            var obj = (GameObject)Activator.CreateInstance(type);

            obj.Transform.Position = position;
            obj.StartPosition = position;
            obj.NeedsLayerSort = true;
            obj.DisplayName = displayName;
            obj.CreateLater();

            LayerManager.AddInstance(obj, layerName);
            return obj;
        }

        /// <summary>
        /// Perform a deep copy of an object in XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T DeepCopyXML<T>(T input)
        {
            using var stream = new MemoryStream();

            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, input);
            stream.Position = 0;
            return (T)serializer.Deserialize(stream);
        }
    }
}