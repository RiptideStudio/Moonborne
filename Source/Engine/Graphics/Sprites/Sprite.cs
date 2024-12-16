using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Game.Objects;
using System.ComponentModel;

namespace Moonborne.Graphics
{
    public class Sprite
    {
        public Texture2D Texture { get; set; }
        public GameObject Parent { get; set; }

        public Color Color { get; set; } = Color.White;
        public int FrameHeight;
        public int FrameWidth;
        public int MaxFrames;
        public SpriteEffects CustomSpriteEffect = SpriteEffects.None;

        /// <summary>
        /// Constructor to create a new sprite
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public Sprite(Texture2D texture = null, GameObject parent = null)
        {
            Texture = texture;
            Parent = parent;
        }

        /// <summary>
        /// Set the spritesheet for animation
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="frameHeight"></param>
        /// <param name="frameWidth"></param>
        /// <param name="maxFrames"></param>
        public void SetSpritesheet(string tex, int frameHeight, int frameWidth, int maxFrames)
        {
            Texture = SpriteManager.GetTexture(tex);
            FrameHeight = frameWidth;
            FrameWidth = frameHeight;
            MaxFrames = maxFrames-1;
        }

        /// <summary>
        /// Main draw event
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            int row = Parent.Frame / (Texture.Width / FrameWidth);
            int column = Parent.Frame % (Texture.Width / FrameWidth);
            Rectangle sourceRect = new Rectangle(column * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight);

            if (Texture != null)
            {
                spriteBatch.Draw(Texture, position, sourceRect, Color, Parent.Rotation, Vector2.Zero, Parent.Scale, CustomSpriteEffect, Parent.Depth);
            }
        }
    }
}