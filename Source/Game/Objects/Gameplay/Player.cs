using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Graphics.Sprites;
using Moonborne.Game.Components;
using System.Collections.Generic;
using Moonborne.Game.Objects;

namespace Moonborne.Game.Gameplay
{
    public class Player : GameObject
    {
        public override void Create()
        {
            sprite.Texture = SpriteManager.GetTexture("Thumbnail");
        }

        public override void Update(float dt)
        {

        }
    }
}