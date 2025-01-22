using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Collisions.Layers;
using Moonborne.Engine.Components;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Inventory;
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
        Down,
        None
    }

    /// <summary>
    /// Extension of game object class. Has better interaction capabilities and more properties
    /// </summary>
    public abstract class Actor : GameObject
    {
        public bool Interactable = false; // Track interactable objects
        public bool Interacted = false;
        public bool InteractingWith = false; // If we are currently interacting with this object

        public int Health = 5; // Default health is 10
        public int MaxHealth = 5;
        public int Damage = 1;
        public bool Friendly = true; // If we are friendly
        public int InvincibilityFrames = 10;
        public bool IsHurt = false;
        public int HealthbarWidth = 32;
        public int HealthbarHeight = 4;
        public Layer OurLayer = null; // This is the layer the object is currently attached to

        public Direction Direction = Direction.Down;
        public State State = State.Idle;

        public Sprite[,] Sprites = new Sprite[Enum.GetValues<State>().Length, Enum.GetValues<Direction>().Length];

        public List<Item> ItemsToDrop = new List<Item>();
        public int InteractDistance { get; set; } = 32;

        /// <summary>
        /// Base actor constructor: we always want a physics component on these
        /// </summary>
        public Actor() : base()
        {
        }

        /// <summary>
        /// Set a sprite for a state and direction
        /// </summary>
        /// <param name="name"></param>
        /// <param name="frameWidth"></param>
        /// <param name="frameHeight"></param>
        /// <param name="state"></param>
        /// <param name="direction"></param>
        public void SetSprite(string name, int frameWidth, int frameHeight, State state, Direction direction)
        {
            // Sprites[(int)state, (int)direction] = SpriteManager.AssignSprite(name, frameWidth, frameHeight);
        }

        /// <summary>
        /// Get a sprite from sprites
        /// </summary>
        /// <param name="state"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Sprite GetSprites(State state, Direction direction)
        {
            return Sprites[(int)state,(int)direction];
        }

        /// <summary>
        /// Called when the game begins
        /// </summary>
        public virtual void OnBeginPlay()
        {

        }

        /// <summary>
        /// Extend the create method
        /// </summary>
        public override void Create()
        {
            base.Create();
            AddComponent(Physics = new Physics(this));
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
                Vector2 healthbarPosition = Transform.Position + new Vector2(-16, -16);
                SpriteManager.DrawRectangle(healthbarPosition, HealthbarWidth, HealthbarHeight, Color.Black);
                SpriteManager.DrawRectangle(healthbarPosition, ((float)Health/MaxHealth)* HealthbarWidth, HealthbarHeight, Color.Green);
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
                float distance = MoonMath.Distance(Transform.Position, Player.Instance.Transform.Position);
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
            foreach (Item item in ItemsToDrop)
            {

            }

            Destroy();
        }
    }
}