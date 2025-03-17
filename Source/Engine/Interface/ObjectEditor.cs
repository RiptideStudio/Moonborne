/*
 * Author: Callen Betts (RiptideDev) 
 * Breif: Defines an inspector to display information about objects
 */

using ImGuiNET;
using Moonborne.Game.Objects;
using System.Reflection;
using System;
using Moonborne.Game.Room;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Moonborne.Graphics.Camera;
using Moonborne.Input;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Moonborne.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Game.Objects.Prefabs;
using System.Xml.Linq;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Assets;

namespace Moonborne.Engine.UI
{
    public static class ObjectEditor
    {
        public static string WindowName = "Prefabs";
        public static GameObject newObject = null;
        public static bool PrefabSelectTypeWindowOpen = false;

        public static void Draw()
        {

        }
    }
}
