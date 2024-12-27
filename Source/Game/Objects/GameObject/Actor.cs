using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Moonborne.Game.Gameplay;
using Moonborne.Graphics;
using Moonborne.Input;
using Moonborne.Utils.Math;
using System;

namespace Moonborne.Game.Objects
{
    /// <summary>
    /// Extension of game object class. Has better interaction capabilities and more properties
    /// </summary>
    public class Actor : GameObject
    {
        public bool Interactable = false; // Track interactable objects
        public bool Interacted = false;
        public bool InteractingWith = false; // If we are currently interacting with this object
        public int InteractDistance = 48;

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
        }
    }
}