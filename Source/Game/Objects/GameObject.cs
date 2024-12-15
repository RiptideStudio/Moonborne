using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Graphics.Sprites;
using Moonborne.Game.Components;
using System.Collections.Generic;

namespace Moonborne.Game.Objects
{
    public abstract class GameObject
    {
        public Sprite sprite; // Sprite object to hold drawing data
        public Texture2D texture;

        public Vector2 position; // Object position
        public Vector2 scale; // Object scale
        public float rotation; // Object rotation

        /// <summary>
        /// Constructor
        /// </summary>
        public GameObject()
        {
            sprite = new Sprite();
            Create();
        }

        /// <summary>
        /// Called when an object is created
        /// </summary>
        public virtual void Create()
        {

        }

        /// <summary>
        /// Called when an object is updated
        /// </summary>
        /// <param name="dt"></param>
        public virtual void Update(float dt)
        {

        }

        /// <summary>
        /// Called when an object is drawn. Draws base sprite by default
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch,position);
        }

        /// <summary>
        /// Called when an object is destroyed
        /// </summary>
        public virtual void Destroy()
        {

        }
    }
}