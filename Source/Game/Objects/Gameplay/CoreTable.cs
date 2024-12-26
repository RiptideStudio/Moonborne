using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Graphics;
using Moonborne.Game.Objects;
using Moonborne.Input;
using Moonborne.Graphics.Camera;
using System;

namespace Moonborne.Game.Gameplay
{
    public class CoreTable : GameObject
    {
        public override void Create()
        {
            base.Create();
            SpriteIndex = SpriteManager.GetSprite("CoreTable");
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }
    }
}