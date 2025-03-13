
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
        public string Name;
        public IntPtr Icon;

        public Texture2D Data => SpriteManager.GetRawTexture(Name);
    }
}