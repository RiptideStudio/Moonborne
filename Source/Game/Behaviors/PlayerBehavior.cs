
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Engine.Components;
using Moonborne.Game.Behavior;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Objects;
using Moonborne.Game.Projectiles;
using Moonborne.Graphics;
using Moonborne.Graphics.Camera;
using Moonborne.Graphics.Window;
using Moonborne.Input;

namespace Moonborne.Game.Components
{
    public class PlayerBehavior : GameBehavior
    {
        internal override string Name => "Player Behavior";
        private Direction Direction = Direction.Down;
        private State State = State.Idle;

        /// <summary>
        /// There should only be one player behavior, this is marked as global player
        /// </summary>
        public override void OnBeginPlay()
        {
            Player.Instance = Parent;
            Player.Instance.Transform = Parent.GetComponent<Transform>();
        }

        /// <summary>
        /// Movement
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            bool moving = false;
            Physics Physics = Parent.GetComponent<Physics>();

            if (InputManager.KeyDown(Keys.W))
            {
                Physics.SetVelocityY(-Physics.Speed);
                Direction = Direction.Up;
                moving = true;
            }
            if (InputManager.KeyDown(Keys.S))
            {
                Physics.SetVelocityY(Physics.Speed);
                Direction = Direction.Down;
                moving = true;
            }
            if (InputManager.KeyDown(Keys.A))
            {
                Physics.SetVelocityX(-Physics.Speed);
                moving = true;
                Direction = Direction.Left;
            }
            if (InputManager.KeyDown(Keys.D))
            {
                Physics.SetVelocityX(Physics.Speed);
                moving = true;
                Direction = Direction.Right;
            }

            if (moving)
            {
                State = State.Move;
            }
            else
            {
                State = State.Idle;
            }
        }
    }
}
