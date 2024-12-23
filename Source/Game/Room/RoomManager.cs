
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Moonborne.Game.Room
{
    public static class RoomManager
    {
        public static Room CurrentRoom;
        public static Dictionary<string, Room> Rooms = new Dictionary<string, Room>();

        /// <summary>
        /// Loads all rooms at the start of the game. Stores them into a map for access
        /// </summary>
        public static void LoadRooms(GraphicsDevice graphicsDevice, MGame game)
        { 
        }

        /// <summary>
        /// Render the current room
        /// </summary>
        public static void Draw()
        {

        }
    }
}