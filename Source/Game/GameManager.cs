using Moonborne.Game.Gameplay;
using Moonborne.Game.Room;
using Moonborne.Graphics.Camera;

namespace Moonborne.Engine
{
    public static class GameManager
    {
        /// <summary>
        /// Called when the game is started (or toggled from editor mode)
        /// </summary>
        public static void Start()
        {
            Camera.SetTarget(Player.Instance);
            RoomEditor.InEditor = false;
        }

        /// <summary>
        /// Stop the game and go back into editor mode
        /// </summary>
        public static void Stop()
        {
            Camera.SetTarget(null);
            RoomEditor.InEditor = true;
        }
    }
}