﻿using Moonborne.Engine.Audio;
using Moonborne.Engine.Files;
using Moonborne.Engine.UI;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Objects;
using Moonborne.Game.Room;
using Moonborne.Graphics;
using Moonborne.Graphics.Camera;
using System;

namespace Moonborne.Engine
{
    public static class GameManager
    {
        public static MGame Game;
        public static bool Paused = true;
        public static bool DebugMode = false;

        /// <summary>
        /// Initialize the game manager
        /// </summary>
        /// <param name="game"></param>
        public static void Initialize(MGame game)
        {
            Game = game;

            GameWatcher.StartWatching("Content/Textures");
        }

        /// <summary>
        /// Toggles play mode on and off
        /// </summary>
        public static void ToggleGamemode()
        {
            if (RoomEditor.InEditor)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        /// <summary>
        /// Called when the game is started (or toggled from editor mode)
        /// </summary>
        public static void Start()
        {
            Console.WriteLine("Starting game...");
            SpriteManager.ReloadTextures();
            Save();
            Resume();
            RoomEditor.InEditor = false;
            AudioManager.PauseAllSound(false);
            Camera.CameraSize = Camera.GameCameraScale;
            Camera.SetTarget(Player.Instance);

            // Iterate over each object and call their begin play function
            foreach (GameObject obj in LayerManager.Objects)
            {
                Actor actor = obj as Actor;

                if (actor != null)
                {
                    actor.OnBeginPlay();
                }
            }
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
        /// Stops the game in its entirety
        /// </summary>
        public static void Exit()
        {
            SettingsManager.Save();
            GameWatcher.StopWatching();
            Game.Exit();
        }

        /// <summary>
        /// Saves the game state
        /// </summary>
        public static void Save()
        {
            RoomEditor.CurrentRoom.Save(RoomEditor.CurrentRoom.Name);
            SettingsManager.Save();
            PrefabEditor.ReloadPrefabs();
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