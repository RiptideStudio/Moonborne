using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;

namespace Moonborne.Graphics
{
    public class Animation
    {
        public int Frame;
        public Sprite Sprite { get; private set; }
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        public int TotalFrames { get; private set; }
        public float AnimationSpeed { get; private set; }
        public float ElapsedTime { get; private set; }
        public bool Looping { get; private set; }

        /// <summary>
        /// Construct a new animation
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <param name="frameWidth"></param>
        /// <param name="frameHeight"></param>
        /// <param name="totalFrames"></param>
        /// <param name="animationSpeed"></param>
        /// <param name="looping"></param>
        public Animation(Sprite sprite, int frameWidth, int frameHeight, int totalFrames, float animationSpeed, bool looping)
        {
            Sprite = sprite;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            TotalFrames = totalFrames-1;
            AnimationSpeed = animationSpeed;
            Looping = looping;

            Frame = 0;
            ElapsedTime = 0f;
        }

        /// <summary>
        /// Update the animation (advance frame)
        /// </summary>
        /// <param name="dt"></param>
        public void Play(float dt)
        {
            ElapsedTime += AnimationSpeed*dt;

            if (ElapsedTime >= 1)
            {
                ElapsedTime = 0;
                Frame++;

                if (Frame >= TotalFrames)
                {
                    if (Looping)
                    {
                        Frame = 0;
                    }
                    else
                    {
                        Frame = TotalFrames - 1; // Stay on the last frame
                    }
                }
            }
        }
    }
}