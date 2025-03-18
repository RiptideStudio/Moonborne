
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Engine.Components;
using Moonborne.Game.Objects;
using Moonborne.Graphics;
using Moonborne.Graphics.Camera;
using Moonborne.Graphics.Window;

namespace Moonborne.Game.Behavior
{
    public abstract class GameBehavior : ObjectComponent
    {
        internal override string Name => "Behavior";

        /// <summary>
        /// Update behaviors
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            base.Update(dt);
        }
    }
}
