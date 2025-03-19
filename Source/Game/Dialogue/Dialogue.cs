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
using Moonborne.Game.Assets;
using Moonborne.Game.Gameplay;

namespace Moonborne.UI.Dialogue
{
    public class Dialogue : Asset
    {
        public Dialogue(string Name, string Folder) : base(Name, Folder)
        {
            AssetType = typeof(Dialogue);
            Text.Add("Lorem Ipsum");
        }

        public Dialogue()
        {
        }

        public string Speaker = "You";
        public int TalkSpeed = 1;
        public bool Skip = true;
        public List<string> Text = new List<string>();
        public Action ScriptedEvent = null;
    }

}
