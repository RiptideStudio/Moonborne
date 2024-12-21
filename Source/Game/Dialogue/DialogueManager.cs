﻿/*
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

namespace Moonborne.UI.Dialogue
{
    public static class DialogueManager
    {
        // Global variables for controlling dialogue
        public static int CharacterIndex { get; set; } = 0;
        public static int LineIndex {  get; set; } = 0;
        public static int DialogueBoxWidth {  get; set; } = 700;
        public static int DialogueBoxHeight {  get; set; } = 192;
        public static float TimeElapsed { get; set; } = 0;
        public static bool Open { get; set; } = false;
        public static bool SpeakerIsPlayer { get; set; } = true; // If the speaker is our player, draw portrait and name differently
        public static bool WaitingForNextLine { get; set; } = false;
        public static string DisplayText { get; set; } = ""; // The text that is currently being displayed on the screen
        public static string TargetText { get; set; } = ""; // The text that is currently being displayed on the screen
        public static string Speaker { get; set; } = "";
        public static Vector2 FontScale { get; set; } = new Vector2(1,1);
        public static Vector2 RootPosition { get; } = new Vector2(32,32);
        public static Vector2 NameOffset { get; set; } = new Vector2(16,16);
        public static Vector2 TextOffset { get; set; } = new Vector2(16,36);
        public static Dictionary<string, Dialogue> Dialogue { get; set; } = new Dictionary<string, Dialogue>(); // Keep track of dialogue
        public static Dialogue ActiveDialogue { get; set; }

        /// <summary>
        /// Load all dialogue data on game start
        /// </summary>
        public static void LoadDialogue()
        {
            // Path to the raw Textures folder
            string dialogueDirectory = "Content/Data/Dialogue";

            if (!Directory.Exists(dialogueDirectory))
            {
                throw new DirectoryNotFoundException($"Textures directory not found at: {dialogueDirectory}");
            }

            // Retrieve every json file in the dialogue folder
            string[] files = Directory.GetFiles(dialogueDirectory, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    // Only process hjson and json files
                    if (file.EndsWith(".hjson"))
                    {
                        // Deserialze the json object
                        string hjson = File.ReadAllText(file);
                        string json = HjsonValue.Parse(hjson).ToString();

                        DialogueData dialogueData = JsonConvert.DeserializeObject<DialogueData>(json);

                        // Create a new Dialogue object with the json data
                        Dialogue dialogue = new Dialogue(dialogueData);
                        string fileName = Path.GetFileName(file);

                        // Add it to the Dialogue manager's list
                        Dialogue.Add(fileName, dialogue);
                    }
                }
                catch (Exception e)
                {
                    // Failed to load dialogue (usually issue with formatting json)
                    Console.Write(e.ToString());
                    throw new Exception("Error: Dialogue in " + file + " was invalid");
                }
            }
        }

        /// <summary>
        /// Reset all dialogue values to default
        /// </summary>
        public static void ResetDialogue()
        {
            LineIndex = 0;
            CharacterIndex = 0;
            DisplayText = "";
            TimeElapsed = 0;
        }

        /// <summary>
        /// Start dialogue given a string that corresponds to the dialogue
        /// </summary>
        /// <param name="DialogueName"></param>
        public static void StartDialogue(string DialogueName)
        {
            // Update the active dialogue and set the first target text
            string filePath = DialogueName + ".hjson";
            ActiveDialogue = Dialogue[filePath];
            TargetText = ActiveDialogue.Data.Text[0];
            Speaker = ActiveDialogue.Data.Speaker;
            Open = true;

            ResetDialogue();
        }

        /// <summary>
        /// Stop current dialogue
        /// </summary>
        public static void StopDialogue()
        {
            Open = false;
        }

        /// <summary>
        /// Update our dialogue each frame, writing characters to the screen
        /// </summary>
        /// <param name="dt"></param>
        public static void WriteDialogue(float dt)
        {
            // If we have finished updating or display text
            if (CharacterIndex >= TargetText.Length)
            {
                WaitingForNextLine = true;
            }

            // Check if we are done updating text
            if (WaitingForNextLine)
            {
                TimeElapsed = 0;

                // Advance the dialogue to the next frame
                if (InputManager.KeyTriggered(Keys.Space) || InputManager.MouseLeftPressed())
                {
                    AdvanceDialogue();
                }
            }
            else
            {
                // If we have not finished updating our text, add the next character
                TimeElapsed += dt;

                // Add the next character
                if (TimeElapsed >= ActiveDialogue.Data.TalkSpeed * dt)
                {
                    DisplayText += TargetText[CharacterIndex];
                    CharacterIndex++;
                    TimeElapsed = 0;
                }
            }
        }

        /// <summary>
        /// Advance to the next line of dialogue
        /// </summary>
        public static void AdvanceDialogue()
        {
            if (LineIndex >= ActiveDialogue.Data.Text.Count-1)
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
                TargetText = ActiveDialogue.Data.Text[LineIndex];
            }
        }

        /// <summary>
        /// Render the dialogue box and text while open
        /// </summary>
        public static void DrawDialogueBox()
        {
            if (Open)
            {
                SpriteManager.SetDrawAlpha(0.5f);
                SpriteManager.DrawRectangle(RootPosition, DialogueBoxWidth, DialogueBoxHeight, Color.Black);
                SpriteManager.ResetDraw();

                SpriteManager.DrawText(Speaker, RootPosition+NameOffset, FontScale, 0, Color.Yellow, DialogueBoxWidth);
                SpriteManager.DrawText(DisplayText, RootPosition+TextOffset, FontScale, 0, Color.White, DialogueBoxWidth);
            }
        }
    }
}
