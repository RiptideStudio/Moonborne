/*
 * Author: Callen Betts (2024)
 * Description: Keeps track of the current dialogue
 */

using Moonborne.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Hjson;
using Newtonsoft.Json;
using System.Linq;
using Moonborne.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moonborne.Game.Objects;
using Moonborne.Utils.Math;
using Moonborne.Game.Room;
using Moonborne.Engine.UI;
using Moonborne.Game.Gameplay;

namespace Moonborne.UI.Dialogue
{
    public static class DialogueManager
    {
        // Global variables for controlling dialogue
        public static int CharacterIndex { get; set; } = 0;
        public static int LineIndex {  get; set; } = 0;
        public static int DialogueBoxWidth {  get; set; } = 300;
        public static int DialogueBoxHeight {  get; set; } = 72;
        public static float TimeElapsed { get; set; } = 0;
        public static bool Open { get; set; } = false;
        public static bool SpeakerIsPlayer { get; set; } = true; // If the speaker is our player, draw portrait and name differently
        public static bool WaitingForNextLine { get; set; } = false;
        public static string DisplayText { get; set; } = ""; // The text that is currently being displayed on the screen
        public static string TargetText { get; set; } = ""; // The text that is currently being displayed on the screen
        public static string Speaker { get; set; } = "";
        public static Vector2 FontScale { get; set; } = new Vector2(0.75f,0.75f);
        public static Vector2 NameOffset { get; set; } = new Vector2(16,8);
        public static Vector2 TextOffset { get; set; } = new Vector2(16,24);
        public static Dialogue ActiveDialogue { get; set; }
        public static GameObject SpeakerObject { get; set; }
        public static Vector2 OriginalPosition { get; set; } = new Vector2(10, 100);
        public static Vector2 RootPosition { get; set; } = OriginalPosition;
        public static Vector2 TargetPosition { get; set; } = OriginalPosition;
        public static float AnimationInterpolation { get; set; } = 0.12f;
        public static GameObject DialogueObject;
        private static float DialogueBoxAlpha = 0f;

        /// <summary>
        /// Create our dialogue object
        /// </summary>
        public static void InitializeLater()
        {
        }

        /// <summary>
        /// Reset all dialogue values to default
        /// </summary>
        public static void ResetDialogue()
        {
            LineIndex = 0;
            CharacterIndex = 0;
            TimeElapsed = 0;
            WaitingForNextLine = false;
        }

        /// <summary>
        /// Write the dialogue if the manager is active/open
        /// </summary>
        /// <param name="dt"></param>
        public static void Update(float dt)
        {
            if (Open)
            {
                WriteDialogue(dt);
                DialogueBoxAlpha = 1;
            }
            else
            {
                DialogueBoxAlpha = 0;
            }

        }

        /// <summary>
        /// Start dialogue given a string that corresponds to the dialogue
        /// </summary>
        /// <param name="DialogueName"></param>
        public static void StartDialogue(Dialogue dialog, GameObject npc)
        {
            if (dialog == null || dialog.Text.Count == 0)
                return;

            ActiveDialogue = dialog;
            DisplayText = "";
            TargetText = ActiveDialogue.Text[0];
            Speaker = ActiveDialogue.Speaker;
            Open = true;

            // If this dialogue is attached to an NPC, set that to be our speaker
            if (npc != null)
            {
                NPCBehavior npcBehavior = npc.GetComponent<NPCBehavior>();
                SpeakerObject = npc;
                npcBehavior.StartTalking();
            }

            ResetDialogue();
        }

        /// <summary>
        /// Stop current dialogue
        /// </summary>
        public static void StopDialogue()
        {
            Open = false;

            // If this dialogue is linked to an NPC, set it's state back to idle
            if (SpeakerObject != null)
            {
                SpeakerObject.GetComponent<NPCBehavior>().StopTalking();
            }

            ResetDialogue();
        }

        /// <summary>
        /// Try to skip dialogue if possible
        /// </summary>
        public static void SkipDialogue()
        {
            if (ActiveDialogue.Skip)
            {
                StopDialogue();
            }
        }

        /// <summary>
        /// Update our dialogue each frame, writing characters to the screen
        /// </summary>
        /// <param name="dt"></param>
        public static void WriteDialogue(float dt)
        {
            int charactersRemaining = TargetText.Length - DisplayText.Length;

            // If we have finished updating or display text
            if (CharacterIndex >= TargetText.Length)
            {
                WaitingForNextLine = true;
            }

            // Check if we are done updating text
            if (WaitingForNextLine)
            {
                TimeElapsed = 0;
            }
            else
            {
                // If we have not finished updating our text, add the next character
                TimeElapsed += dt;

                int talkSpeed = ActiveDialogue.TalkSpeed;

                // Add the next character
                if (TimeElapsed >= talkSpeed * dt)
                {
                    DisplayText += TargetText[CharacterIndex];
                    CharacterIndex++;
                    TimeElapsed = 0;
                }
            }

            // Advance the dialogue to the next frame
            if (InputManager.KeyTriggered(Keys.Space) || InputManager.MouseLeftPressed())
            {
                if (WaitingForNextLine)
                {
                    // Next set of dialogue
                    AdvanceDialogue();
                }
                else
                {
                    // Quick-skip dialogue
                    DisplayText = TargetText;
                    WaitingForNextLine = true;
                }
            }
        }

        /// <summary>
        /// Advance to the next line of dialogue
        /// </summary>
        public static void AdvanceDialogue()
        {
            if (LineIndex >= ActiveDialogue.Text.Count-1)
            {
                // We are done with this dialogue
                StopDialogue();
            }
            else
            {
                // Continue on to the next line
                LineIndex++;
                CharacterIndex = 0;
                TimeElapsed = 0;
                WaitingForNextLine = false;
                DisplayText = "";
                TargetText = ActiveDialogue.Text[LineIndex];
            }
        }

        /// <summary>
        /// Render the dialogue box and text while open
        /// </summary>
        public static void DrawDialogueBox()
        {
            RootPosition = MoonMath.Lerp(RootPosition, TargetPosition, AnimationInterpolation);
            SpriteManager.SetDrawAlpha(DialogueBoxAlpha);
            SpriteManager.DrawRectangle(RootPosition, DialogueBoxWidth, DialogueBoxHeight, Color.Black);

            SpriteManager.DrawText(Speaker, RootPosition+NameOffset, FontScale, 0, Color.Yellow);
            SpriteManager.DrawText(DisplayText, RootPosition+TextOffset, FontScale, 0, Color.White, DialogueBoxWidth-32);
            SpriteManager.ResetDraw();
        }
    }
}
