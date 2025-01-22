
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Game.Objects;

namespace Moonborne.Engine.Components
{
    public abstract class ObjectComponent
    {
        public string Name = "Base Component"; // Display name of the component
        public string Description = "Serves as a base component"; // What this component does
        public GameObject Parent = null;

        /// <summary>
        /// Calls create
        /// </summary>
        public ObjectComponent(GameObject parent)
        {
            Parent = parent;
            Create();
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
        /// Draw a component
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
