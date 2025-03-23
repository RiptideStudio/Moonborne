using ImGuiNET;
using Moonborne.Game.Behaviors;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Moonborn.Game.Behaviors
{
    public class Node
    {
        public int Id;
        public string Title;
        public Vector2 Position;
        public Vector2 Size = new Vector2(200, 100);
        public float PinSize = 8f;
        public List<Pin> Inputs = new();
        public List<Pin> Outputs = new();

        public void DrawNode()
        {
            var drawList = ImGui.GetWindowDrawList();
            var io = ImGui.GetIO();
            Vector2 basePos = Position;

            // Title (centered above box)
            Vector2 titleSize = ImGui.CalcTextSize(Title);
            Vector2 titlePos = basePos + new Vector2((Size.X - titleSize.X) / 2, -titleSize.Y - 6);
            drawList.AddText(titlePos, ImGui.GetColorU32(Vector4.One), Title);

            // Draw node box
            drawList.AddRectFilled(basePos, basePos + Size, ImGui.GetColorU32(new Vector4(0.2f, 0.2f, 0.2f, 1f)), 8);
            drawList.AddRect(basePos, basePos + Size, ImGui.GetColorU32(Vector4.One), 8, 0, 1.0f);

            // Drag to move
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && MouseOverNode(basePos, Size))
            {
                Position += io.MouseDelta;
            }

            // Draw pins at middle left/right
            foreach (var input in Inputs)
            {
                input.Offset = new Vector2(0, Size.Y / 2); // middle-left
                var pinPos = basePos + input.Offset;
                input.ScreenPosition = pinPos;
                drawList.AddCircleFilled(pinPos, PinSize, ImGui.GetColorU32(Vector4.One));
            }

            foreach (var output in Outputs)
            {
                output.Offset = new Vector2(Size.X, Size.Y / 2); // middle-right
                var pinPos = basePos + output.Offset;
                output.ScreenPosition = pinPos;
                drawList.AddCircleFilled(pinPos, PinSize, ImGui.GetColorU32(Vector4.One));
            }
        }

        public static void DrawLink(Vector2 from, Vector2 to)
        {
            var drawList = ImGui.GetWindowDrawList();
            Vector2 cp1 = from + new Vector2(50, 0);
            Vector2 cp2 = to - new Vector2(50, 0);
            drawList.AddBezierCubic(from, cp1, cp2, to, ImGui.GetColorU32(Vector4.One), 2.0f);
        }

        public static bool MouseOverNode(Vector2 position, Vector2 size)
        {
            Vector2 mouse = ImGui.GetIO().MousePos;
            return mouse.X >= position.X && mouse.X <= position.X + size.X &&
                   mouse.Y >= position.Y && mouse.Y <= position.Y + size.Y;
        }

        public static bool IsMouseOverRect(Vector2 pos, Vector2 size)
        {
            Vector2 mouse = ImGui.GetIO().MousePos;
            return mouse.X >= pos.X && mouse.X <= pos.X + size.X &&
                   mouse.Y >= pos.Y && mouse.Y <= pos.Y + size.Y;
        }
    }

    public class Link
    {
        public int OutputPinId;
        public int InputPinId;
    }

    public class Pin
    {
        public int Id;
        public string Name;
        public Vector2 Offset;
        public Vector2 ScreenPosition;
        public static bool IsMouseOverPin(Vector2 pos, float radius = 8f)
        {
            Vector2 mouse = ImGui.GetIO().MousePos;
            return Vector2.Distance(mouse, pos) <= radius;
        }

        public static Vector2 GetPinWorldPosition(Node node, Pin pin)
        {
            return pin.ScreenPosition; // now precalculated
        }
    }
}

