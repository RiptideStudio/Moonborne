/*
 * Author: Callen Betts (2024)
 * Description: Used as game objects that have extended interaction capabilities
 */

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Projectiles;
using Moonborne.Graphics;
using Moonborne.Input;
using Moonborne.UI.Dialogue;
using Moonborne.Utils.Math;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Game.Behavior;
using Moonborne.Engine.Components;

namespace Moonborne.Game.Objects
{
    /// <summary>
    /// Enum for defining states for NPCs (idle, attack, etc)
    /// </summary>
    public enum State
    {
        Idle,
        Wander,
        Move,
        Talking,
        Attack
    }

    public class NPCBehavior : GameBehavior
    {
        internal override string Name => "NPC Behavior";

        public GameObject Target;
        public List<Dialogue> DialogueObjects = new List<Dialogue>();

        public bool CanInteract = true;
        public bool CanTalk = false; 
        public bool Friendly = true; 
        public float AggroDistance = 100f;
        public float WanderDistance = 80f;
        public int WanderTimeMin = 120;
        public int WanderTimeMax = 180;
        public float InteractDistance = 32f;

        private int WanderTime = 150;
        private Vector2 WanderPosition = Vector2.One;
        private float ElapsedTime = 0;
        private State State = State.Idle;
        private int DialogueState = 0;
        private bool InteractingWith = false;

        /// <summary>
        /// Extend the update method 
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            base.Update(dt);

            // Simple state-based updates for NPCs
            switch (State)
            {
                case State.Idle:
                    Idle(dt);
                    break;             
                
                case State.Wander:
                    Wander(dt);
                    break;

                case State.Attack:
                    Attack();
                    break;

                case State.Talking:
                    Talking();
                    break;
            }

            // If we can interact with this NPC
            if (CanInteract)
            {
                float distance = MoonMath.Distance(Parent.Transform.Position, Player.Instance.Transform.Position);

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
        /// Extend the create method
        /// </summary>
        public override void Create()
        {
            base.Create();
        }

        /// <summary>
        /// While the NPC is idling
        /// </summary>
        public virtual void Idle(float dt)
        {
            ElapsedTime += dt;

            // Set a random target position to start wandering around in
            if (ElapsedTime >= WanderTime*dt)
            {
                ElapsedTime = 0;
                WanderTime = MoonMath.RandomRange(WanderTimeMin, WanderTimeMax);

                float angle = MoonMath.RandomRange(0f, 2*MathF.PI);
                Vector2 randomDir = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                WanderPosition = Parent.Transform.Position + randomDir * 48;

                // Return back to our original spawn
                if (MoonMath.Distance(Parent.StartPosition,WanderPosition) >= WanderDistance)
                {
                    WanderPosition = Parent.StartPosition;
                }

                State = State.Wander;
            }
        }

        /// <summary>
        /// Draw extension
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Called while the NPC is wandering around
        /// </summary>
        /// <param name="dt"></param>
        public virtual void Wander(float dt)
        {
            float distanceToTarget = MoonMath.Distance(Parent.Transform.Position, WanderPosition);
            ElapsedTime += dt;

            if (ElapsedTime > WanderTime * dt)
            {
                ElapsedTime = 0;
                State = State.Idle;
            }

            Physics Physics = Parent.GetComponent<Physics>();

            if (distanceToTarget <= Physics.Velocity.Length())
            {
                // Stop wandering
                ElapsedTime = 0;
                State = State.Idle;
            }
            else
            {
                // Walk to target position
                Vector2 targetDirection = MoonMath.Direction(Parent.Transform.Position, WanderPosition);
                targetDirection.Normalize();
                Physics.Velocity = targetDirection * Physics.Speed;
            }
        }

        /// <summary>
        /// While the NPC is attacking
        /// </summary>
        public virtual void Attack()
        {
            if (Target != null && !Friendly)
            {
                Physics Physics = Parent.GetComponent<Physics>();

                Target = Player.Instance;
                Vector2 targetDirection = Target.Transform.Position - Parent.Transform.Position;
                targetDirection.Normalize();
                Physics.Velocity = targetDirection * Physics.Speed;
            }
        }

        /// <summary>
        /// While the NPC is talking
        /// </summary>
        public virtual void Talking()
        {
        }

        /// <summary>
        /// When we leave an interaction
        /// </summary>
        public virtual void LeaveInteract()
        {
            // Stop dialogue if we walk away
            float playerDistance = MoonMath.Distance(Parent.Transform.Position, Player.Instance.Transform.Position);

            if (playerDistance >= InteractDistance)
            {
                DialogueManager.SkipDialogue();
            }
        }

        /// <summary>
        /// Called when an NPC is talked to
        /// </summary>
        public virtual void OnInteract()
        {
            // Some NPCs don't have dialogue. In this case, do nothing
            if (DialogueObjects.Count <= 0 || DialogueState >= DialogueObjects.Count)
                return;

            // Attempt to start dialogue
            if (!DialogueManager.Open)
            {
                // Start dialogue
                Dialogue dialogue = DialogueObjects[DialogueState];
                DialogueManager.StartDialogue(dialogue,Parent);
            }
            else
            {
                // Stop dialogue (quick skip if possible)
                DialogueManager.SkipDialogue();
            }
        }

        /// <summary>
        /// Called when an NPC stops talking
        /// </summary>
        public virtual void StopTalking()
        {
            State = State.Idle;

            // Advance dialogue state
            DialogueState++;
            DialogueState = Math.Clamp(DialogueState, 0, DialogueObjects.Count - 1);
            ElapsedTime = 0;
            InteractingWith = false;
        }        
        
        /// <summary>
        /// Called when an NPC stops talking
        /// </summary>
        public virtual void StartTalking()
        {
            State = State.Talking;
        }
    }
}