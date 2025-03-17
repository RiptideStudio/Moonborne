
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Game.Objects;
using System.Collections.Generic;
using System;

namespace Moonborne.Engine.Components
{
    public abstract class ObjectComponent
    {
        internal string Name = "Base Component"; // Display name of the component
        internal string Description = "Serves as a base component"; // What this component does
        internal GameObject Parent = null;

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

    public class ObjectComponentData
    {
        public string Type { get; set; } // Stores class name (e.g., "SpriteRenderer")
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public ObjectComponent CreateComponent()
        {
            Type componentType = Type.GetType();
            if (componentType == null) return null;

            ObjectComponent component = (ObjectComponent)Activator.CreateInstance(componentType);
            foreach (var property in Properties)
            {
                var propInfo = componentType.GetProperty(property.Key);
                if (propInfo != null)
                {
                    object value = Convert.ChangeType(property.Value, propInfo.PropertyType);
                    propInfo.SetValue(component, value);
                }
            }
            return component;
        }
    }

}
