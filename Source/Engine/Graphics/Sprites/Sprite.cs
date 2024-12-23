using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Game.Objects;
using Moonborne.Graphics.Window;
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
        public int LayerDepth = 0;
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
        public void Draw(SpriteBatch spriteBatch, int frame, Vector2 position, Vector2 scale, float rotation, Color color)
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
                Vector2 origin = new Vector2(FrameWidth / 2f, FrameHeight / 2f);

                spriteBatch.Draw(Texture, position, sourceRect, Color, rotation, origin, scale, CustomSpriteEffect, LayerDepth);
            }
        }
    }
}