﻿
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Engine.Collision;
using Moonborne.Game.Objects;
using Moonborne.Graphics;
using Moonborne.Input;
using System;

namespace Moonborne.Engine.UI
{
    public class UIElement : GameObject
    {
        public bool Hover = false;
        Color IdleColor = Color.LightGray;
        Color HoverColor = Color.White;
        Color ClickColor = Color.Gray;
        public Action ClickAction;
        public string Text;

        /// <summary>
        /// Add this ui element to a manager so we can get and remove elements
        /// </summary>
        public UIElement() : base()
        {
        }

        /// <summary>
        /// UI elements do not get auto added to the layer manager
        /// </summary>
        public override void Create()
        {

        }

        /// <summary>
        /// Draw this button
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            SpriteManager.DrawSetAlignment(true);
            SpriteManager.DrawText(Text, Transform.Position, Transform.Scale, Transform.Rotation, GetComponent<Sprite>().Tint);
            SpriteManager.DrawSetAlignment(false);
            Update(MGame.DeltaTime);
        }

        /// <summary>
        /// Logical operations such as checking hover, etc.
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            if (!SpriteIndex.Visible)
                return;

            // Update our sprite, hitbox, and animation
            base.Update(dt);

            // Hover over the element
            if (InputManager.MouseIsHovering(Hitbox))
            {
                OnHover();
            }
            else
            {
                OnMouseExit();
            }

            // Click the element
            if (InputManager.MouseLeftReleased())
            {
                if (Hover)
                {
                    OnClick();
                }
            }
        }

        /// <summary>
        /// Called while we are hovering over this element
        /// </summary>
        public virtual void OnHover()
        {
            if (!Hover)
            {
                // When our mouse enters for the first time
                Hover = true;
                OnMouseEnter();
            }
            GetComponent<Sprite>().Tint = HoverColor;

            WhileMouseHeld();
        }

        /// <summary>
        /// When we enter the UI element
        /// </summary>
        public virtual void OnMouseEnter()
        {

        }

        /// <summary>
        /// When our mouse leaves the element
        /// </summary>
        public virtual void OnMouseExit()
        {
            Hover = false;
            GetComponent<Sprite>().Tint = IdleColor;
        }

        /// <summary>
        /// When we click a UI element
        /// </summary>
        public virtual void OnClick()
        {
            ClickAction?.Invoke();
        }

        /// <summary>
        /// Called while the element is being pressed down
        /// </summary>
        public virtual void WhileMouseHeld()
        {
            if (Hover && InputManager.MouseLeftDown())
            {
                GetComponent<Sprite>().Tint = ClickColor;
            }
        }
    }
}