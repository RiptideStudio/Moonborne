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

        public enum Axis
        {
            Horizontal,
            Vertical,
            None
        }

        public Color Color { get; set; } = Color.White;
        public int FrameHeight = 16;
        public int FrameWidth = 16;
        public int MaxFrames = 1;
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
        /// Flips the sprite a given direction
        /// </summary>
        public void Flip(Axis dir = Axis.None)
        {
            switch (dir)
            {
                case Axis.Horizontal:
                    CustomSpriteEffect = SpriteEffects.FlipHorizontally;
                    break;                
                
                case Axis.Vertical:
                    CustomSpriteEffect = SpriteEffects.FlipVertically;
                    break;

                case Axis.None:
                    CustomSpriteEffect = SpriteEffects.None;
                    break;
            }
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
            MaxFrames = maxFrames;
        }

        /// <summary>
        /// Main draw event, draws a sprite given parameters
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, int frame, Vector2 position, Vector2 scale, float rotation=0.0f, int layer=0)
        {
            // Only draw if texture is valid
            if (Texture != null)
            {
                // Wrap back around if our frame goes too high
                if (frame >= MaxFrames)
                {
                    frame = frame % MaxFrames;
                }

                int row = frame / (Texture.Width / FrameWidth);
                int column = frame % (Texture.Width / FrameWidth);
                Rectangle sourceRect = new Rectangle(column * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight);

                spriteBatch.Draw(Texture, position, sourceRect, Color, rotation, Vector2.Zero, scale, CustomSpriteEffect, layer);
            }
        }
    }
}