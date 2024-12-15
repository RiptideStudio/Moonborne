using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;

namespace Moonborne.Graphics.Sprites
{
    public class Sprite
    {
        public Texture2D Texture { get; set; }
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Constructor to create a new sprite
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public Sprite(Texture2D texture = null)
        {
            Texture = texture;
        }

        /// <summary>
        /// Main draw event
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (Texture != null)
            {
                spriteBatch.Draw(Texture, position, Color);
            }
        }
    }
}