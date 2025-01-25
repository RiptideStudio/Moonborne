
using System;
using System.Collections.Generic;
using System.IO;
using FMOD;

namespace Moonborne.Engine.Audio
{
    /// <summary>
    /// If audio is a sound or music
    /// </summary>
    public enum AudioType
    {
        Sound,
        Music
    }

    /// <summary>
    /// Sound IDs for easier and safer references
    /// </summary>
    public enum SoundID
    {
        Earthward,
        CoreTable,
        Home
    }


    public static class AudioManager
    {
        public static Dictionary<string, GameSound> Sounds = new Dictionary<string, GameSound>(); // List of all sounds
        public static FMOD.System AudioSystem;
        public static FMOD.ChannelGroup MainChannel;
        public static int MaxChannels { get; private set; } = 32;
        public static FMOD.Channel Sound {  get; private set; }
        public static FMOD.Channel Music {  get; private set; }

        /// <summary>
        /// Initialze the audio system and load all sounds
        /// </summary>
        public static void Initialize()
        {
            FMOD.RESULT result = FMOD.Factory.System_Create(out AudioSystem);
            if (result != FMOD.RESULT.OK)
            {
                throw new Exception($"Fmod result was {result}");
            }
            AudioSystem.init(MaxChannels, INITFLAGS.NORMAL,(IntPtr)0);
            result = AudioSystem.createChannelGroup("MainChannel", out MainChannel);
            if (result != FMOD.RESULT.OK)
            {
                throw new Exception($"Fmod result was {result}");
            }

            // Load all files that are OGG or MP3
            string contentFolderPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"Content\Audio"));
            string[] files = Directory.GetFiles(contentFolderPath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (file.EndsWith(".ogg") || file.EndsWith(".mp3"))
                {
                    // Create the sound object
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string directory = Path.GetDirectoryName(file);

                    AudioType audioType = AudioType.Sound;

                    if (directory == "Music")
                    {
                        audioType = AudioType.Music;
                    }

                    result = AudioSystem.createSound(file, MODE.DEFAULT, out FMOD.Sound sound);
                    GameSound gameSound = new GameSound(fileName, sound, audioType);

                    Sounds.Add(fileName, gameSound);
                }
            }

            PlaySound(SoundID.Home);
            PauseAllSound(true);
        }

        /// <summary>
        /// Update the audio system each frame
        /// </summary>
        /// <param name="dt"></param>
        public static void Update(float dt)
        {
            AudioSystem.update();
        }

        /// <summary>
        /// Play a sound given a sound ID
        /// </summary>
        /// <param name="sound"></param>
        public static void PlaySound(SoundID sound)
        {
            GameSound soundObject = Sounds.TryGetValue(sound.ToString(), out var gameSound) ? gameSound : null;

            if (soundObject != null)
            {
                soundObject.Play();
            }
        }

        /// <summary>
        /// Play a sound given a string
        /// </summary>
        /// <param name="sound"></param>
        public static void PlaySound(string sound)
        {
            GameSound soundObject = Sounds.TryGetValue(sound, out var gameSound) ? gameSound : null;

            if (soundObject != null)
            {
                soundObject.Play();
            }
        }

        /// <summary>
        /// Stops all audio playing
        /// </summary>
        public static void PauseAllSound(bool val)
        {
            MainChannel.setPaused(val);
        }
    }
}