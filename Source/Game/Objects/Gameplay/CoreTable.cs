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
using ImGuiNET;

namespace Moonborne.Game.Gameplay
{
    public class CoreTable : Actor
    {
        private Vector2 DisplayPosition = new Vector2(120,120);
        public override void Create()
        {
            base.Create();
            SpriteIndex = GetSprite("CoreTable");
            Interactable = true;
            InteractDistance = 32;
            Collideable = false;
        }

        public override void DrawUI(SpriteBatch spriteBatch)
        {
            if (InteractingWith)
            {
                DrawRectangle(DisplayPosition, 400, 160, Color.Black);
                DrawText("Core Table", DisplayPosition, new Vector2(2, 2), 0, Color.White);

                if (ImGui.Button("Upgrade Gun"))
                {
                    Player.Instance.Gun.Upgrade();
                }
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