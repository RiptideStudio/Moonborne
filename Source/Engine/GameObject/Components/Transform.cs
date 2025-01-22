
using Microsoft.Xna.Framework;
using Moonborne.Game.Objects;

namespace Moonborne.Engine.Components
{
    public class Transform : ObjectComponent
    {
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; }
        public Vector2 OldPosition;

        public Transform(GameObject parent) : base(parent)
        {

        }

        public override void Create()
        {
            Name = "Transform";
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
        }
    }
}
