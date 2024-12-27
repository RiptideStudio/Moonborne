using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Graphics;
using Moonborne.Game.Objects;
using Moonborne.Input;
using Moonborne.Graphics.Camera;
using System;

using static Moonborne.Graphics.SpriteManager;
using Microsoft.Xna.Framework.Audio;
using Moonborne.Engine.Audio;

namespace Moonborne.Game.Gameplay
{
    public class CoreTable : Actor
    {

        public override void Create()
        {
            base.Create();
            SpriteIndex = SpriteManager.GetSprite("CoreTable");
            Interactable = true;
            InteractDistance = 32;
        }

        public override void DrawUI(SpriteBatch spriteBatch)
        {
            if (InteractingWith)
            {
                DrawRectangle(new Rectangle(16,16,380,100),Color.Black);
            }
        }

        public void ToggleCoreStation()
        {
            AudioManager.PlaySound(SoundID.CoreTable);
        }

        public override void OnInteract()
        {
            ToggleCoreStation();
        }

        public override void LeaveInteract()
        {
            ToggleCoreStation();
        }
    }
}