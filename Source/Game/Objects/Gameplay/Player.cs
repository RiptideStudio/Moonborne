using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Graphics;
using Moonborne.Game.Objects;
using Moonborne.Input;
using Moonborne.Graphics.Camera;
using Moonborne.Game.Projectiles;
using Moonborne.Game.Inventory;
using Moonborne.Game.Room;

namespace Moonborne.Game.Gameplay
{
    public class Player : GameObject
    {
        public Gun Gun { get; set; }
        public static Player Instance { get; private set; }
        public string DisplayName { get; set; } = "Player";
        public override void Create()
        {
            SpriteIndex = SpriteManager.GetSprite("HornetIdle");
            AnimationSpeed = 10;
            Scale = new Vector2(1, 1);
            Position.X = 400;
            Position.Y = 400;
            Speed = 100;
            MaxSpeed = 100;
            Instance = this;

            Camera.SetTarget(this);
            Camera.SetPosition(Position);
            Gun = new Gun(this,250f,10);
            LayerManager.AddInstance(Gun, "Object");
            ObjectLibrary.CreateObject<NPC>(Position);
            ObjectLibrary.CreateObject<CoreTable>(Position,"Tiles");
            ObjectLibrary.CreateObject<LunarCore>(new Vector2(32,32)+Position);
            ObjectLibrary.CreateObject<LunarCore>(new Vector2(0,32)+Position);
            ObjectLibrary.CreateObject<LunarCore>(new Vector2(-32,32)+Position);
            ObjectLibrary.CreateObject<LunarCore>(new Vector2(-32,32)+Position);
            ObjectLibrary.CreateObject<LunarCore>(new Vector2(-64,32)+Position);
            ObjectLibrary.CreateObject<LunarCore>(new Vector2(-96,32)+Position);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawShadow(Position.X,Position.Y+20,6,2);
            base.Draw(spriteBatch);
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            // Shoot a test projectile
            if (InputManager.KeyDown(Keys.C))
            {
                Gun.Shoot();
            }

            if (InputManager.KeyDown(Keys.W))
            {
                Velocity.Y = -Speed;
                SpriteIndex = SpriteManager.GetSprite("HornetIdle");
            }
            if (InputManager.KeyDown(Keys.A))
            {
                SpriteIndex.Flip(Sprite.Axis.Horizontal);
                Velocity.X = -Speed;
            }
            if (InputManager.KeyDown(Keys.S))
            {
                Velocity.Y = Speed;
            }
            if (InputManager.KeyDown(Keys.D))
            {
                SpriteIndex.Flip(Sprite.Axis.None);
                Velocity.X = Speed;
            }
        }
    }
}