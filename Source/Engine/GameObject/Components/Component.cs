using Moonborne.Game.Objects;

namespace Moonborne.Engine.Components
{
    public abstract class ObjectComponent : BaseGameBehavior
    {
        internal abstract string Name { get; } // Display name of the component
        internal string Description = "Serves as a base component"; // What this component does
        internal GameObject Parent = null;
        public bool Visible = true;
        public bool VisibleInGame = true;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ObjectComponent() : base()
        {

        }
    }
}
