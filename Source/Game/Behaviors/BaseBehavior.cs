using Microsoft.Xna.Framework.Graphics;
using Moonborne.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonborne.Game.Objects
{
    public abstract class BaseGameBehavior
    {
        /// <summary>
        /// Calls create, default constructor
        /// </summary>
        public BaseGameBehavior()
        {

        }

        /// <summary>
        /// Called when a component is created
        /// </summary>
        public virtual void Create()
        {

        }

        /// <summary>
        /// Update a component
        /// </summary>
        /// <param name="dt"></param>
        public virtual void Update(float dt)
        {

        }

        /// <summary>
        /// Destroy a component
        /// </summary>
        public virtual void Destroy()
        {

        }

        /// <summary>
        /// Called when the game starts (play is pressed)
        /// </summary>
        public virtual void OnBeginPlay()
        {

        }

        /// <summary>
        /// Draw a component
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
