using Moonborne.Engine.Audio;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Room;
using Moonborne.Graphics.Camera;
using System;

namespace Moonborne.Engine
{
    public static class GameManager
    {
        public static bool Paused = true;
        public static bool DebugMode = false;

        /// <summary>
        /// Called when the game is started (or toggled from editor mode)
        /// </summary>
        public static void Start()
        {
            Console.WriteLine("Starting game...");
            Resume();
            Camera.Zoom = Camera.DefaultZoom;
            Camera.TargetZoom = Camera.DefaultZoom;
            Camera.SetTarget(Player.Instance);
            RoomEditor.InEditor = false;
            AudioManager.PauseAllSound(false);
        }

        /// <summary>
        /// Stop the game and go back into editor mode
        /// </summary>
        public static void Stop()
        {
            Console.WriteLine("Stopping game...");
            Pause();
            Camera.SetTarget(null);
            RoomEditor.InEditor = true;
            AudioManager.PauseAllSound(true);
        }

        /// <summary>
        /// Pause the game
        /// </summary>
        public static void Pause()
        {
            Console.WriteLine("Paused game");

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
            Console.WriteLine("Resumed game");

            if (Paused)
            {
                Paused = false;
            }
        }
    }
}