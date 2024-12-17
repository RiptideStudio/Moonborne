using Moonborne.Graphics;
using System;
using System.Numerics;

namespace Moonborne.Game.Dialogue
{
    public static class DialogueManager
    {
        public static int TalkSpeed = 1;
        public static bool Open = false;
        public static string DisplayText;
        public static void StartDialogue()
        {
            Open = true;
        }
        public static void StopDialogue()
        {
            Open = false;
        }

        public static void DrawDialogueBox()
        {
            if (Open)
            {
                SpriteManager.DrawText(DisplayText, new Vector2(160, 320), new Vector2(1, 1), 0, null);
            }
        }
    }
}
