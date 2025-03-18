/*
 * Author: Callen Betts (callen.bettsvirott@digipen.edu)
 * Moonborne Engine 2025
 * 
 * Brief: Defines a prefab class to be edited in-editor
 * 
 */

using Force.DeepCloner;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Engine.Components;
using Moonborne.Game.Assets;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Room;
using Moonborne.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Moonborne.Game.Objects.Prefabs
{
    public class Prefab : Asset, IComponentContainer
    {
        [JsonConverter(typeof(ObjectComponentConverter))]
        public List<ObjectComponent> Components { get; private set; } = new List<ObjectComponent>();

        public Prefab(string name, string folder) : base(name, folder)
        {
            IsDraggable = true;
            AssetType = typeof(EmptyObject);
        }

        /// <summary>
        /// Adds a component to this object
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public void AddComponent(ObjectComponent component)
        {
            Components.Add(component);
            component.Create();
        }

        /// <summary>
        /// Remove a component
        /// </summary>
        /// <param name="component"></param>
        public void RemoveComponent(ObjectComponent component)
        {
            Components.Remove(component);
        }

        /// <summary>
        /// Instantiate a prefab
        /// </summary>
        /// <param name="position"></param>
        public GameObject Instantiate(Vector2 position)
        {
            GameObject gameObject = (GameObject)Activator.CreateInstance(AssetType);

            foreach (ObjectComponent component in Components)
            {
                gameObject.AddComponent(component.DeepClone());
            }

            Transform transform = gameObject.GetComponent<Transform>();
            if (transform != null)
            {
                transform.Position = position;
            }

            LayerManager.AddInstance(gameObject, RoomEditor.selectedLayer);

            return gameObject;
        }

        /// <summary>
        /// Get a component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : ObjectComponent
        {
            return Components.OfType<T>().FirstOrDefault();
        }
    }
}
