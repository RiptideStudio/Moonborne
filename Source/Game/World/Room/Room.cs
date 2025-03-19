
using FMOD;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Tiled;
using Moonborne.Engine;
using Moonborne.Engine.Components;
using Moonborne.Engine.UI;
using Moonborne.Game.Assets;
using Moonborne.Game.Objects;
using Moonborne.Game.Objects.Prefabs;
using System;

namespace Moonborne.Game.Room
{
    public class Room : Asset
    {
        public Room(string Name, string Folder) : base(Name, Folder) 
        {

        }

        public Room()
        {

        }

        /// <summary>
        /// Save a room to a json file made of multiple tilemaps
        /// </summary>
        /// <param name="name"></param>
        /// <param name="overridePath"></param>
        public void Save(string name, string overridePath = null)
        {
            Console.WriteLine($"Saved Room '{name}'");
        }

        /// <summary>
        /// Load this room into the world
        /// </summary>
        public void Load()
        {
            GameManager.WorldState.LoadJsonIntoWorld(Name);
            RoomEditor.CurrentRoom = this;
        }

        /// <summary>
        /// Open the room
        /// </summary>
        public override void OnDoubleClick()
        {
            Load();
        }
    }
}