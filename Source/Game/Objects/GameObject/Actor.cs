using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonborne.Game.Gameplay;
using Moonborne.Graphics;
using Moonborne.Input;
using Moonborne.Utils.Math;
using System;
using System.Collections.Generic;

namespace Moonborne.Game.Objects
{
    
    /// <summary>
    /// Sprites are directional
    /// </summary>
    public enum Direction
    {
        Right,
        Left,
        Up,
        Down
    }

    /// <summary>
    /// Extension of game object class. Has better interaction capabilities and more properties
    /// </summary>
    public class Actor : GameObject
    {
        public bool Interactable = false; // Track interactable objects
        public bool Interacted = false;
        public bool InteractingWith = false; // If we are currently interacting with this object
        public int InteractDistance = 48;

        public int Health = 5; // Default health is 10
        public int MaxHealth = 5;
        public int Damage = 1;
        public bool Friendly = true; // If we are friendly
        public int InvincibilityFrames = 10;
        public bool IsHurt = false;
        public Direction Direction = Direction.Down;

        public Dictionary<Direction, Sprite> IdleSprites = new Dictionary<Direction, Sprite>(); // Use the sprite index associated with direction we are in
        public Dictionary<Direction, Sprite> WalkSprites = new Dictionary<Direction, Sprite>(); // When we are walking


        /// <summary>
        /// Extend the create method
        /// </summary>
        public override void Create()
        {
            base.Create();
        }

        /// <summary>
        /// Called when the actor is interacted with
        /// </summary>
        public virtual void OnInteract()
        {

        }

        /// <summary>
        /// Called when walking away from an interactable object
        /// </summary>
        public virtual void LeaveInteract()
        {

        }

        /// <summary>
        /// Extend the draw method 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            
            if (IsHurt)
            {
                Vector2 healthbarPosition = Position + new Vector2(0, -16);
                SpriteManager.DrawRectangle(healthbarPosition, 36, 8, Color.Black);
                SpriteManager.DrawRectangle(healthbarPosition, ((float)Health/MaxHealth)*36, 8, Color.Green);
            }
        }

        /// <summary>
        /// Extend the update method to include checks for interaction
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            base.Update(dt);    

            // Interact with this actor if we are close to it
            if (Interactable)
            {
                float distance = MoonMath.Distance(Position, Player.Instance.Position);
                if (distance < InteractDistance) 
                {
                    if (InputManager.KeyTriggered(Keys.E))
                    {
                        OnInteract();
                        InteractingWith = !InteractingWith;
                    }
                }
                else if (InteractingWith)
                {
                    LeaveInteract();
                    InteractingWith = false;
                }
            }

            // Update spritesheet based on direction
            if (IdleSprites.Count > 0)
            {
                SpriteIndex = IdleSprites[Direction];
            }
        }

        /// <summary>
        /// Damage an actor
        /// </summary>
        /// <param name="damage"></param>
        public virtual void Hurt(int damage)
        {
            Health -= damage;
            IsHurt = true;

            if (Health <= 0)
            {
                Kill();
            }
        }

        /// <summary>
        /// Called when an actor reaches 0 health
        /// </summary>
        public virtual void Kill()
        {
            Destroy();
        }
    }
}