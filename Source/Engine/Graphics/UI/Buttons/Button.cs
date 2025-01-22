using Microsoft.Xna.Framework;
using Moonborne.Graphics;
using System;

namespace Moonborne.Engine.UI
{
    public class Button : UIElement
    {
        /// <summary>
        /// Create a UI element with a given position and scale
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        public Button(Vector2 position, Vector2 scale, string text = "", Action action = null) : base()
        {
            Transform.Scale = scale;
            Transform.Position = position;
            SpriteIndex.SetSpritesheet("Button");
            ClickAction = action;
            Text = text;
        }
    }
}