/*
 * Author: Callen Betts (callen.bettsvirott@digipen.edu)
 * Moonborne Engine 2025
 * 
 * Brief: Defines a prefab class to be edited in-editor
 * 
 */

using Microsoft.Xna.Framework.Graphics;
using Moonborne.Engine.Components;
using Moonborne.Game.Assets;
using Moonborne.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Moonborne.Game.Objects.Prefabs
{
    public class Prefab : Asset
    {
        public List<ObjectComponentData> Components;

        public Prefab(string name, string folder) : base(name, folder)
        {
        }

        /// <summary>
        /// Instantiate a prefab
        /// </summary>
        /// <param name="position"></param>
        public void Instantiate(Vector2 position)
        {
            GameObject gameObject = (GameObject)Activator.CreateInstance(AssetType);
            gameObject.Transform.Position = position;

            foreach (ObjectComponentData component in Components)
            {
                gameObject.AddComponent(component.CreateComponent());
            }
        }
    }
}
