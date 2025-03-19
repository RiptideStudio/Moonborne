using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Engine.Components;
using Moonborne.Graphics;

namespace Moonborne.Engine.Graphics.Lighting
{
    public class Light : ObjectComponent
    {
        internal override string Name => "Point Light";

        public SpriteTexture Sprite;
        public Vector2 Position;
        public Color Color = Color.White;
        public float Intensity = 1f;
        public float Radius = 64f;

        /// <summary>
        /// Lights are by default using the light texture
        /// </summary>
        public Light() : base() 
        {
            Sprite = SpriteManager.GetTexture("Light");
        }
    }
}
