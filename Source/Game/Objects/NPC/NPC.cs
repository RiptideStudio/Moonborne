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
        public State PreviousState { get; set; } = State.Idle;
        public int DialogueState { get; set; } = 0; // Allows us to cycle through dialogue
        public Dictionary<int, string> Dialogue { get; set; } = new Dictionary<int, string>(); // List of our Dialogue objects
        public bool CanInteract { get; set; } = true; // Default, we can interact
        public bool CanTalk { get; set; } = false; 
        public float AggroDistance { get; set; } = 100f;
        public float WanderDistance { get; set; } = 80f;
        public float ElapsedTime { get; set; } = 0;
        public int WanderTimeMin { get; set; } = 120;
        public int WanderTimeMax { get; set; } = 180;
        public int WanderTime { get; set; } = 150;
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
            WanderPosition = Position;
            Friendly = false;
            Speed = 33;
            Dialogue[0] = "Test";
            Dialogue[1] = "Test2";

            SetSprite("NpcIdleLeft", 16, 16, State.Idle, Direction.Left);
            SetSprite("NpcIdleRight", 16, 16, State.Idle, Direction.Right);
            SetSprite("NpcIdleDown", 16, 16, State.Idle, Direction.Down);
            SetSprite("NpcIdleUp", 16, 16, State.Idle, Direction.Up);
            SpriteIndex = GetSprites(State.Idle, Direction.Down);
        }

        /// <summary>
        /// While the NPC is idling
        /// </summary>
        public virtual void Idle(float dt)
        {
            InteractWithNpc();

            ElapsedTime += dt;

            // Set a random target position to start wandering around in
            if (ElapsedTime >= WanderTime*dt)
            {
                ElapsedTime = 0;
                WanderTime = MoonMath.RandomRange(WanderTimeMin, WanderTimeMax);

                float angle = MoonMath.RandomRange(0f, 2*MathF.PI);
                Vector2 randomDir = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                WanderPosition = Position + randomDir * 48;

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
            DrawShadow(Position.X, Position.Y+5, 6, 2);
        }

        /// <summary>
        /// Called while the NPC is wandering around
        /// </summary>
        /// <param name="dt"></param>
        public virtual void Wander(float dt)
        {
            float distanceToTarget = MoonMath.Distance(Position, WanderPosition);

            if (distanceToTarget <= Velocity.Length())
            {
                // Stop wandering
                State = State.Idle;
            }
            else
            {
                // Walk to target position
                Vector2 targetDirection = MoonMath.Direction(Position, WanderPosition);
                targetDirection.Normalize();
                Velocity = targetDirection * Speed;
            }

            InteractWithNpc();
        }

        /// <summary>
        /// Helper function for talking to NPC
        /// </summary>
        public void InteractWithNpc()
        {
            // Attempt to interact with the NPC
            float playerDistance = MoonMath.Distance(Position, Player.Instance.Position);

            if (playerDistance <= InteractDistance)
            {
                if (InputManager.KeyTriggered(Keys.E))
                {
                    OnInteract();
                }
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
                Vector2 targetDirection = Target.Position-Position;
                targetDirection.Normalize();
                Velocity = targetDirection * Speed;
            }
        }

        /// <summary>
        /// While the NPC is talking
        /// </summary>
        public virtual void Talking()
        {
            // Talk to NPC
            InteractWithNpc();
        }

        public override void LeaveInteract()
        {
            base.LeaveInteract();

            // Stop dialogue if we walk away
            float playerDistance = MoonMath.Distance(Position, Player.Instance.Position);

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
            if (Dialogue.Count == 0 || DialogueState >= Dialogue.Count)
            {
                return;
            }

            // Attempt to start dialogue
            if (!DialogueManager.Open)
            {
                // Start dialogue
                string dialogue = Dialogue[DialogueState];
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
            DialogueState = Math.Clamp(DialogueState, 0, Dialogue.Count - 1);
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