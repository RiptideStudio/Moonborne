/*
 * Author: Callen Betts (2024)
 * Description: Used as a container for dialogue text and other properties
 */

using Moonborne.Graphics;
using System;
using System.Numerics;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.Drawing;

namespace Moonborne.UI.Dialogue
{
    /// <summary>
    /// Define a dialogue type for better use
    /// </summary>
    public enum DialogueType
    {
        Test,
        Test2
    }

    // Define the json structure for dialogue
    public class DialogueData
    {
        public string Speaker { get; set; }
        public List<string>Text { get; set; }
        public int TalkSpeed { get; set; } = 1;
        public bool Skip { get; set; } = false;
        public Action ScriptedEvent { get; set; }
    }

    public class Dialogue
    {
        // Keep track of the dialogue data
        public DialogueData Data;

        public string Name = "None";

        // Eventually turn this into a dictionary of dictionaries
        // There will be some special escape sequences that define special effects
        // Like "*Bold Text*" and "**Italics**"
        public Dictionary<string, string> DialogueText = new Dictionary<string, string>();

        /// <summary>
        /// The text for this dialogue
        /// </summary>
        /// <param name="dialogueText"></param>
        public Dialogue(DialogueData data)
        {
            Data = data;
        }

        public Dialogue()
        {

        }
    }
}
