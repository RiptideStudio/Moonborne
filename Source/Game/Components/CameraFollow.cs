
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Game.Objects;
using Moonborne.Graphics;
using Moonborne.Graphics.Camera;
using Moonborne.Graphics.Window;

namespace Moonborne.Engine.Components
{
    public class CameraFollow : ObjectComponent
    {
        internal override string Name => "Camera";

        public float CameraSize = 1; // The value we use in our game
        public static int CameraWidth;
        public static int CameraHeight;

        public CameraFollow() : base()
        {
            VisibleInGame = false;
            Description = "Follows the target object";
        }

        /// <summary>
        /// Set our camera's target to be this
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            if (Parent != null)
            {
                Camera.Target = Parent;
                Camera.CameraSize = CameraSize;
            }
        }

        /// <summary>
        /// Draw our camera rectangle
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            CameraWidth = (int)(WindowManager.BaseViewportWidth * CameraSize);
            CameraHeight = (int)(WindowManager.BaseViewportHeight * CameraSize);

            Transform Transform = Parent.GetComponent<Transform>();
            Rectangle outline = new Rectangle((int)Transform.Position.X - CameraWidth / 2, (int)Transform.Position.Y - CameraHeight / 2, CameraWidth, CameraHeight);
            SpriteManager.DrawRectangle(outline, Color.White, true);
            SpriteManager.DrawSprite("Camera", 0, Transform.Position - new Vector2(0, 32), Vector2.One, 0f, Color.White);
        }
    }
}
