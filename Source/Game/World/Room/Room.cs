
using FMOD;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Tiled;
using Moonborne.Engine.Components;
using Moonborne.Engine.UI;
using Moonborne.Game.Objects;
using Moonborne.Game.Objects.Prefabs;
using Moonborne.Graphics;
using Moonborne.Graphics.Camera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Moonborne.Game.Room
{
    public class Room
    {
        public string Name { get; set; } = "Room";

        /// <summary>
        /// Save a room to a json file made of multiple tilemaps
        /// </summary>
        /// <param name="name"></param>
        /// <param name="overridePath"></param>
        public void Save(string name, string overridePath = null)
        {
            Console.WriteLine($"Saved Room '{name}'");
        }
    }
}