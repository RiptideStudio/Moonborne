using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Graphics;
using Moonborne.Game.Objects;

namespace Moonborne.Game.Gameplay
{
    public class Pillar : GameObject
    {
        public override void Create()
        {
            base.Create();
            SpriteIndex = SpriteManager.GetSprite("Pillar");
            IsStatic = true;
        }
    }
}