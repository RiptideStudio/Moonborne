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
        private Vector2 DisplayPosition = new Vector2(120,120);
        private Vector2 ButtonSize = new Vector2(1,1);
        private Button UpgradeButton;
        private int Cost = 3; // How much it costs to upgrade the gun

        public override void Create()
        {
            base.Create();
            SpriteIndex = GetSprite("CoreTable");
            Interactable = true;
            InteractDistance = 32;
            Collideable = false;

            UpgradeButton = new Button(new Vector2(320,160), ButtonSize, "Upgrade Gun", () => UpgradeGun());

        }

        public override void DrawUI(SpriteBatch spriteBatch)
        {
            if (InteractingWith)
            {
                DrawRectangle(DisplayPosition, 400, 160, Color.Black);
                DrawText("Core Table", DisplayPosition, new Vector2(2, 2), 0, Color.White);
                UpgradeButton.Draw(spriteBatch);
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