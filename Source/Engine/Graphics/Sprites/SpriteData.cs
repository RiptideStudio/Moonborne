
using Microsoft.Xna.Framework.Graphics;

namespace Moonborne.Graphics
{
    public class SpriteTexture
    {
        public int FrameWidth;
        public int FrameHeight;
        public int MaxFrames = 1;
        public Texture2D Data;
        public SpriteTexture(Texture2D texture)
        {
            FrameWidth = texture.Width;
            FrameHeight = texture.Height;
        }
    }
}