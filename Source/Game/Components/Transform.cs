using Microsoft.Xna;
using System.Buffers.Text;
using System.Numerics;

namespace Moonborne.Game.Components
{
    public class Transform : Component
    {
        private Vector2 position;
        private Vector2 scale;
        private float rotation;
        
        public override string Name => "Transform";
        public override ComponentType Type => ComponentType.Transform;
    }
}