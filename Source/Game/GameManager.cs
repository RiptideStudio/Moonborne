using Moonborne.Game.Gameplay;
using Moonborne.Game.Room;
using Moonborne.Graphics.Camera;

namespace Moonborne.Engine
{
    public static class GameManager
    {
        public static bool Paused = true;
        public static bool DebugMode = true;

        /// <summary>
        /// Called when the game is started (or toggled from editor mode)
        /// </summary>
        public static void Start()
        {
            Resume();
            Camera.Zoom = Camera.DefaultZoom;
            Camera.TargetZoom = Camera.DefaultZoom;
            Camera.SetTarget(Player.Instance);
            RoomEditor.InEditor = false;
        }

        /// <summary>
        /// Stop the game and go back into editor mode
        /// </summary>
        public static void Stop()
        {
            Pause();
            Camera.SetTarget(null);
            RoomEditor.InEditor = true;
        }

        /// <summary>
        /// Pause the game
        /// </summary>
        public static void Pause()
        {
            if (!Paused)
            {
                Paused = true;
            }
        }

        /// <summary>
        /// Resume a paused game
        /// </summary>
        public static void Resume()
        {
            if (Paused)
            {
                Paused = false;
            }
        }
    }
}