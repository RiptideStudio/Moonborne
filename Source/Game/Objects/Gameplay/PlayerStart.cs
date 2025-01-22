using Moonborne.Graphics;
using Moonborne.Game.Objects;
using Moonborne.Graphics.Camera;
using Moonborne.Game.Room;

namespace Moonborne.Game.Gameplay
{
    public class PlayerStart : Actor
    {
        public override void CreateLater()
        {
            base.CreateLater();
            SpriteIndex.Texture = SpriteManager.GetTexture("GamepadIcon");
            SpriteIndex.VisibleInGame = false;
        }

        /// <summary>
        /// Spawn the player here
        /// </summary>
        public override void OnBeginPlay()
        {
            // Spawn a new player here if valid
            if (Player.Instance == null)
            {
                Player player = new Player();
                Player.Instance = player;
                LayerManager.AddInstance(player, "Player");
            }

            Camera.Target = Player.Instance;
            Player.Instance.Transform.Position = Transform.Position;
        }
    }
}