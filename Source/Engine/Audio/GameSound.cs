
using FMOD;
using SharpDX.Multimedia;
using System.Collections.Generic;

namespace Moonborne.Engine.Audio
{
    public class GameSound
    {
        public Sound Sound { get; set; }
        public string Name { get; set; }
        public float Volume { get; set; } = 1f;
        public float Pitch { get; set; } = 1f;
        public bool Loop { get; set; } = false;
        public AudioType AudioType { get; set; }

        /// <summary>
        /// Construct a new game sound
        /// </summary>
        /// <param name="name"></param>
        public GameSound(string name, FMOD.Sound sound, AudioType audioType)
        {
            Name = name;
            Sound = sound;
            AudioType = audioType;

            // Auto-set loop for music
            if (AudioType == AudioType.Music)
            {
                Loop = true;
            }
        }

        /// <summary>
        /// Play a sound with given parameters
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <param name="loop"></param>
        public void Play(float volume = 1f, float pitch = 1f, bool loop = false)
        {
            FMOD.RESULT result = AudioManager.AudioSystem.playSound(Sound,AudioManager.MainChannel,false,out FMOD.Channel channel);
        }
    }
}