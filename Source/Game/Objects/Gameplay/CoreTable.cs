using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Graphics;
using Moonborne.Game.Objects;
using Moonborne.Engine.Audio;

using static Moonborne.Graphics.SpriteManager;
using Moonborne.Engine.UI;
using Moonborne.Game.Inventory;

namespace Moonborne.Game.Gameplay
{
    public class CoreTable : Actor
    {
        private Vector2 DisplayPosition = new Vector2(320,160);
        private Vector2 ButtonSize = new Vector2(1,1);
        private Button UpgradeButton;
        private Button ExitButton;
        private int Cost = 3; // How much it costs to upgrade the gun

        public override void Create()
        {
            base.Create();
            SpriteIndex = GetSprite("CoreTable");
            Interactable = true;
            InteractDistance = 32;
            Collideable = false;

            UpgradeButton = new Button(new Vector2(320,160), ButtonSize, "Upgrade Gun", () => UpgradeGun());
            ExitButton = new Button(new Vector2(320,210), ButtonSize, "Exit", () => Exit());
        }

        public override void DrawUI(SpriteBatch spriteBatch)
        {
            if (InteractingWith)
            {
                SetDrawAlpha(0.75f);
                DrawSetAlignment(true);
                DrawRectangle(DisplayPosition, 400, 240, Color.Black);
                SetDrawAlpha(1);
                DrawText("Core Table", DisplayPosition+new Vector2(0,-60), new Vector2(2, 2), 0, Color.White);
                DrawSetAlignment(false);
                UpgradeButton.Draw(spriteBatch);
                ExitButton.Draw(spriteBatch);
            }
        }

        public void UpgradeGun()
        {
            if (InventoryManager.HasItem(typeof(LunarCore), Cost))
            {
                Player.Instance.Gun.Upgrade();
                InventoryManager.RemoveItem(typeof(LunarCore), Cost);
            }
        }

        public void Exit()
        {
            ToggleCoreStation();
            InteractingWith = false;
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