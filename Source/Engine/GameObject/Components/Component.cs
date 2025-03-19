using Moonborne.Game.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Moonborne.Engine.Components
{
    public abstract class ObjectComponent : BaseGameBehavior
    {
        internal abstract string Name { get; } // Display name of the component
        internal string Description = "Serves as a base component"; // What this component does
        internal GameObject Parent = null;
        public bool Visible = true;
        public bool VisibleInGame = true;
        private static List<ObjectComponent> allComponents = new List<ObjectComponent>();

        /// <summary>
        /// Default constructor
        /// </summary>
        protected ObjectComponent() : base()
        {
            allComponents.Add(this);
        }

        ~ObjectComponent()
        {
            allComponents.Remove(this);
        }

        public static List<T> GetAllComponents<T>() where T : ObjectComponent
        {
            return allComponents.OfType<T>().ToList();
        }
    }
}
