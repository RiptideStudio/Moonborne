/*
 * Author: Callen Betts (2024)
 * Description: Moon core currency item
 */

using Moonborne.Graphics;

namespace Moonborne.Game.Inventory
{
    public class LunarCore : Item
    {
        public override void Create()
        {
            base.Create();
            SpriteIndex.Texture = SpriteManager.GetTexture("MoonCore");
            Name = "Star Core";
        }
    }
}