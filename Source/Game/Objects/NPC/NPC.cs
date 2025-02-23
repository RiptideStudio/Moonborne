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

    public class NPC : Lifeform
    {
        public Gun Gun { get; set; }
        public State PreviousState = State.Idle;
        public int DialogueState = 0; // Allows us to cycle through dialogue
        public List<Dialogue> DialogueObjects { get; set; } = new List<Dialogue>();
        public bool CanInteract { get; set; } = true; // Default, we can interact
        public bool CanTalk = false; 
        public float AggroDistance { get; set; } = 100f;
        public float WanderDistance { get; set; } = 80f;
        public float ElapsedTime { get; set; } = 0;
        public int WanderTimeMin { get; set; } = 120;
        public int WanderTimeMax { get; set; } = 180;
        public int WanderTime = 150;
        public Vector2 WanderPosition = Vector2.One;
        public GameObject Target { get; set; }


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
        }

        /// <summary>
        /// Extend the create method
        /// </summary>
        public override void Create()
        {
            base.Create();
            WanderPosition = Transform.Position;
            Friendly = false;
            Interactable = true;
            Physics.Speed = 33;

            SetSprite("NpcIdleLeft", 16, 16, State.Idle, Direction.Left);
            SetSprite("NpcIdleRight", 16, 16, State.Idle, Direction.Right);
            SetSprite("NpcIdleDown", 16, 16, State.Idle, Direction.Down);
            SetSprite("NpcIdleUp", 16, 16, State.Idle, Direction.Up);

            SetSprite("PlayerWalkLeft", 16, 16, State.Wander, Direction.Left);
            SetSprite("PlayerWalkRight", 16, 16, State.Wander, Direction.Right);
            SetSprite("PlayerWalkDown", 16, 16, State.Wander, Direction.Down);
            SetSprite("PlayerWalkUp", 16, 16, State.Wander, Direction.Up);
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
                WanderPosition = Transform.Position + randomDir * 48;

                // Return back to our original spawn
                if (MoonMath.Distance(StartPosition,WanderPosition) >= WanderDistance)
                {
                    WanderPosition = StartPosition;
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
            float distanceToTarget = MoonMath.Distance(Transform.Position, WanderPosition);
            ElapsedTime += dt;

            if (ElapsedTime > WanderTime * dt)
            {
                ElapsedTime = 0;
                State = State.Idle;
            }

            if (distanceToTarget <= Physics.Velocity.Length())
            {
                // Stop wandering
                ElapsedTime = 0;
                State = State.Idle;
            }
            else
            {
                // Walk to target position
                Vector2 targetDirection = MoonMath.Direction(Transform.Position, WanderPosition);
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
                Target = Player.Instance;
                Vector2 targetDirection = Target.Transform.Position - Transform.Position;
                targetDirection.Normalize();
                Physics.Velocity = targetDirection * Physics.Speed;
            }
        }

        /// <summary>
        /// While the NPC is talking
        /// </summary>
        public virtual void Talking()
        {
            // Talk to NPC
        }

        public override void LeaveInteract()
        {
            base.LeaveInteract();

            // Stop dialogue if we walk away
            float playerDistance = MoonMath.Distance(Transform.Position, Player.Instance.Transform.Position);

            if (playerDistance >= InteractDistance)
            {
                DialogueManager.SkipDialogue();
            }
        }

        /// <summary>
        /// Called when an NPC is talked to
        /// </summary>
        public override void OnInteract()
        {
            base.OnInteract();

            // Some NPCs don't have dialogue. In this case, do nothing
            if (DialogueObjects.Count == 0 || DialogueState >= DialogueObjects.Count)
            {
                return;
            }

            // Attempt to start dialogue
            if (!DialogueManager.Open)
            {
                // Start dialogue
                string dialogue = DialogueObjects[DialogueState].Name;
                DialogueManager.StartDialogue(dialogue,this);
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