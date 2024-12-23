
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moonborne.Graphics;
using Moonborne.Input;
using Microsoft.Xna.Framework.Input;

namespace Moonborne.Game.Room
{
    public static class RoomEditor
    {
        public static bool DebugDraw = true;
        public static int SelectedTile = 0;
        public static int[,] grid = new int[100,100];
        public static Tilemap Tilemap;

        /// <summary>
        /// Create a default tilemap
        /// </summary>
        public static void Initialize()
        {
            Tilemap = new Tilemap(SpriteManager.GetTexture("JungleTileset"), new int[100,100],16,10);
        }

        /// <summary>
        /// Draw tiles in the world
        /// </summary>
        public static void DrawTiles(SpriteBatch spriteBatch)
        {
            Tilemap.DrawGrid(spriteBatch);
        }

        /// <summary>
        /// Draw tileset editor
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawEditor(SpriteBatch spriteBatch)
        {
            Tilemap.DrawTilesetPreview(spriteBatch,32,32);
            Tilemap.HandleTileSelection(32, 32);

            if (InputManager.KeyDown(Keys.LeftControl))
            {

                if (InputManager.KeyTriggered(Keys.S))
                {
                    Tilemap.Save(Tilemap.Name);
                }                
                
                if (InputManager.KeyTriggered(Keys.L))
                {
                    Tilemap.Load(Tilemap.Name);
                }
            }
        }
    }
}