
using Microsoft.Xna.Framework.Graphics;
using System;
using Newtonsoft.Json;
using Moonborne.Game.Assets;

namespace Moonborne.Graphics
{
    public class SpriteTexture : Asset
    {
        public int FrameWidth;
        public int FrameHeight; // Height of a single frame
        public int TextureWidth;
        public int TextureHeight; // Height of actual texture
        public int MaxFrames = 1;

        [JsonIgnore]
        public IntPtr Icon;

        [JsonIgnore]
        public Texture2D Data;

        public SpriteTexture(string Name, string Folder) : base(Name, Folder) 
        {
            PostLoad();
        }

        /// <summary>
        /// Re-load texture data that isn't serialized
        /// </summary>
        public override void PostLoad()
        {
            Data = SpriteManager.GetRawTexture(Name);
            Icon = SpriteManager.GetImGuiTexture(Name);
        }
    }
}