using Moonborne.Graphics;
using Moonborne.Game.Objects;
using Moonborne.Graphics.Camera;

namespace Moonborne.Game.Gameplay
{
    public class PlayerStart : GameObject
    {
        public override void CreateLater()
        {
            base.CreateLater();
            SpriteIndex = SpriteManager.GetSprite("GamepadIcon");
            VisibleInGame = false;
            Player.Instance.Position = Position;
            Camera.TargetPosition = Position;
            Camera.Position = Position;
        }

    }
}