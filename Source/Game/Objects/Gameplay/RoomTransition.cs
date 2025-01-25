using Moonborne.Graphics;
using Moonborne.Game.Objects;
using Moonborne.Graphics.Camera;
using Moonborne.Game.Room;

namespace Moonborne.Game.Gameplay
{
    public class RoomTransition : Actor
    {
        public string TargetRoom { get; set; } = "Room";
        private bool Used = false;

        public override void Create()
        {
            base.Create();
            SpriteIndex.SetSpritesheet("RoomTransition");
            SpriteIndex.VisibleInGame = false;
        }

        /// <summary>
        /// Load the next room
        /// </summary>
        /// <param name="other"></param>
        public override void OnCollisionStart(GameObject other)
        {
            // If we collide with the player, transition to target room
            if (other is Player)
            {
                // Check if the room exists, and if it does go to it
                if (RoomManager.Rooms.ContainsKey(TargetRoom) && !Used)
                {
                    Used = true;
                    RoomManager.SetActiveRoom(RoomManager.Rooms[TargetRoom]);
                }
            }
        }

    }
}