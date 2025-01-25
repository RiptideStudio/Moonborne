
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Moonborne.Graphics
{
    public class SpriteTexture
    {
        public int FrameWidth;
        public int FrameHeight; // Height of a single frame
        public int TextureWidth;
        public int TextureHeight; // Height of actual texture
        public int MaxFrames = 1;
        public Texture2D Data;
        public IntPtr Icon;
        public string Name; // Name of this texture

        public SpriteTexture(Texture2D texture)
        {
        }
    }
}