using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Engine;
using Moonborne.Engine.Components;
using Moonborne.Game.Objects;
using Moonborne.Game.Room;
using Moonborne.Graphics.Window;
using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Moonborne.Graphics
{
    public class Sprite : ObjectComponent
    {
        internal override string Name => "Sprite 2D";

        public enum Axis
        {
            Horizontal,
            Vertical,
            None
        }

        public SpriteTexture Texture = null;
        public int MaxFrames = 1;
        public int AnimationSpeed = 10;
        public float Alpha = 1f;

        public Color Color = Color.White;
        public int Frame = 0;
        internal float LayerDepth = 0;
        internal float FrameTime = 0;
        public Vector2 DrawOffset = Vector2.Zero;

        public SpriteEffects CustomSpriteEffect = SpriteEffects.None;

        /// <summary>
        /// Constructor to create a new sprite
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public Sprite(SpriteTexture texture = null, GameObject parent = null) : base()
        {
            Texture = texture;
            Parent = parent;
        }

        /// <summary>
        /// Default
        /// </summary>
        public Sprite() : base()
        {
            Description = "Stores object's sprite and texture data";
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
        public void SetSpritesheet(string tex, int frameHeight, int frameWidth)
        {
            Texture = SpriteManager.GetTexture(tex);
            Texture.FrameHeight = frameWidth;
            Texture.FrameWidth = frameHeight;
            MaxFrames = Texture.Data.Width/Texture.Data.Height; // Horizontal spritesheets
        }

        /// <summary>
        /// Simple texture set
        /// </summary>
        /// <param name="tex"></param>
        public void SetSpritesheet(string tex)
        {
            Texture = SpriteManager.GetTexture(tex);

            if (Texture != null)
            {
                MaxFrames = 1;
            }
        }

        /// <summary>
        /// Update any attached animations
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            FrameTime += AnimationSpeed * dt;

            if (FrameTime > 1)
            {
                FrameTime = 0;
                Frame++;
            }

            if (Frame >= MaxFrames)
            {
                Frame = 0;
            }
        }

        /// <summary>
        /// Draw the sprite
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Not visible flags
            if (!Visible || Texture.Data == null)
                return;

            if (!GameManager.Paused && !VisibleInGame)
                return;

            Transform Transform = Parent.GetComponent<Transform>();

            Parent.Hitbox.X = Parent.HitboxXOffset + (int)Transform.Position.X - Parent.Hitbox.Width / 2;
            Parent.Hitbox.Y = Parent.HitboxYOffset + (int)Transform.Position.Y - Parent.Hitbox.Height / 2;

            Parent.Hitbox.Width = Texture.FrameWidth - Parent.HitboxWidthOffset;
            Parent.Hitbox.Height = Texture.FrameHeight - Parent.HitboxHeightOffset;
            Parent.Hitbox.Height = (int)(Parent.Hitbox.Height*Transform.Scale.Y);
            Parent.Hitbox.Width = (int)(Parent.Hitbox.Width * Transform.Scale.X);

            LayerDepth = Parent.Depth;

            DrawSprite(spriteBatch, Frame, Transform.Position + DrawOffset, Transform.Scale, Transform.Rotation, Color);
        }

        /// <summary>
        /// Main draw event, draws a sprite given parameters
        /// </summary>
        public void DrawSprite(SpriteBatch spriteBatch, int frame, Vector2 position, Vector2 scale, float rotation, Color color)
        {
            // Only draw if texture is valid
            if (Texture != null)
            {
                // Wrap back around if our frame goes too high
                if (frame >= MaxFrames)
                {
                    frame = frame % MaxFrames;
                }

                int row = frame / (Texture.TextureWidth / Texture.FrameWidth);
                int column = frame % (Texture.TextureWidth / Texture.FrameWidth);
                Rectangle sourceRect = new Rectangle(column * Texture.FrameWidth, row * Texture.FrameHeight, Texture.FrameWidth, Texture.FrameHeight);
                Vector2 origin = new Vector2(Texture.FrameWidth / 2f, Texture.FrameHeight / 2f);

                spriteBatch.Draw(Texture.Data, position, sourceRect, color, rotation, origin, scale, CustomSpriteEffect, LayerDepth);
            }
        }
    }
}