using Moonborne.Game.Objects;
using Moonborne.Graphics;

namespace Moonborne.Game.Gameplay
{
    public class EmptyObject : Actor
    {
        public override void Create()
        {
            base.Create();
            SpriteIndex = SpriteManager.GetSprite("QuestionMark");
            VisibleInGame = false;
        }
    }
}