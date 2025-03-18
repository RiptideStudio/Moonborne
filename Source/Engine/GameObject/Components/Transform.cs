
using Microsoft.Xna.Framework;
using Moonborne.Game.Objects;

namespace Moonborne.Engine.Components
{
    public class Transform : ObjectComponent
    {
        internal override string Name => "Transform";

        public Vector2 Position;
        public Vector2 Scale = Vector2.One;
        public float Rotation;
        internal Vector2 OldPosition;

        public Transform() : base()
        {
            Description = "Stores object's transform data (position, scale, rotation)";
        }

        /// <summary>
        /// Update our position and old position
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            // Update our position and velocity
            OldPosition = Position;

            // Update our quick pointer
            if (Parent != null)
            {
                Parent.Transform = this;
            }
        }
    }
}
