using Moonborne.Game.Objects;
using Moonborne.Graphics;

namespace Moonborne.Game.Gameplay
{
    public class EmptyObject : Actor
    {
        public override void Create()
        {
            base.Create();
            SpriteIndex.Texture = SpriteManager.GetTexture("QuestionMark");
            SpriteIndex.VisibleInGame = false;
        }
    }
}