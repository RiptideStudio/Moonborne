/*
 * Author: Callen Betts (callen.bettsvirott@digipen.edu)
 * Moonborne Engine 2025
 * 
 * Brief: Defines a prefab class to be edited in-editor
 * 
 */

using Microsoft.Xna.Framework.Graphics;
using Moonborne.Engine.Components;
using Moonborne.Graphics;
using System.Collections.Generic;

namespace Moonborne.Game.Objects.Prefabs
{
    public class Prefab : GameObject
    {

        /// <summary>
        /// Add sprite and transform by default
        /// </summary>
        public Prefab()
        {
            AddComponent(new Sprite(this));
            AddComponent(new Transform(this));
        }



    }
}
