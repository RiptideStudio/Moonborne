using ImGuiNET;
using Moonborne.Game.Behaviors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Moonborn.Game.Behaviors
{
    public class Node
    {
        public int Id;
        public string Title;
        public Vector2 Position;
        public Vector2 Size = new Vector2(200, 100);
        public Color BoxColor = Color.DarkSlateGray;
        public Color OutlineColor = Color.SlateGray;
        public float PinSize = 8f;
        public List<Pin> Inputs = new();
        public List<Pin> Outputs = new();

        private static Color DarkenColor(Color color, float factor)
        {
            factor = Math.Clamp(factor, 0f, 1f); // Ensure factor is within range
            return Color.FromArgb(
                color.A,
                (int)(color.R * factor),
                (int)(color.G * factor),
                (int)(color.B * factor)
            );
        }

        public void DrawNode(Vector2 screenPos, float zoomLevel)
        {
            var drawList = ImGui.GetWindowDrawList();
            var io = ImGui.GetIO();
            Vector2 basePos = screenPos;

            // Check hover
            bool isHovered = Node.MouseOverNode(screenPos, Size * zoomLevel);

            // Darken box color if hovered
            Color boxColor = isHovered ? DarkenColor(BoxColor, 0.75f) : BoxColor;
            uint boxColorU32 = (uint)boxColor.ToArgb();
            uint outlineColorU32 = (uint)OutlineColor.ToArgb();

            // Title
            Vector2 titleSize = ImGui.CalcTextSize(Title);
            Vector2 titlePos = basePos + new Vector2((Size.X * zoomLevel - titleSize.X) / 2, -titleSize.Y - 6 * zoomLevel);
            drawList.AddText(titlePos, ImGui.GetColorU32(Vector4.One), Title);

            // Draw node box
            Vector2 scaledSize = Size * zoomLevel;
            drawList.AddRectFilled(basePos, basePos + scaledSize, boxColorU32, 8 * zoomLevel);
            drawList.AddRect(basePos, basePos + scaledSize, outlineColorU32, 8 * zoomLevel, 0, 4f * zoomLevel);

            // Draw pins
            foreach (var input in Inputs)
            {
                input.Offset = new Vector2(0, scaledSize.Y / 2);
                var pinPos = basePos + input.Offset;
                input.ScreenPosition = pinPos;
                drawList.AddCircleFilled(pinPos, PinSize * zoomLevel, ImGui.GetColorU32(Vector4.One));
            }

            foreach (var output in Outputs)
            {
                output.Offset = new Vector2(scaledSize.X, scaledSize.Y / 2);
                var pinPos = basePos + output.Offset;
                output.ScreenPosition = pinPos;
                drawList.AddCircleFilled(pinPos, PinSize * zoomLevel, ImGui.GetColorU32(Vector4.One));
            }
        }

        public static void DrawLink(Vector2 from, Vector2 to)
        {
            var drawList = ImGui.GetWindowDrawList();
            // Adjust control points based on distance between nodes
            float distance = Vector2.Distance(from, to);
            float controlPointOffset = Math.Min(50, distance * 0.4f);
            
            Vector2 cp1 = from + new Vector2(controlPointOffset, 0);
            Vector2 cp2 = to - new Vector2(controlPointOffset, 0);
            
            // Adjust line thickness based on zoom level
            float lineThickness = 2.0f;
            if (ImGui.GetIO().KeyCtrl) // Assuming we can access zoom level through a global or pass it as parameter
            {
                // This is a placeholder - in real implementation, you'd use the actual zoom level
                float zoomLevel = 1.0f;
                lineThickness *= zoomLevel;
            }
            
            drawList.AddBezierCubic(from, cp1, cp2, to, ImGui.GetColorU32(Vector4.One), lineThickness);
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
        public static bool IsMouseOverPin(Vector2 pos, float zoomLevel = 1.0f)
        {
            Vector2 mouse = ImGui.GetIO().MousePos;
            float scaledRadius = 8f * zoomLevel;
            return Vector2.Distance(mouse, pos) <= scaledRadius;
        }

        public static Vector2 GetPinWorldPosition(Node node, Pin pin)
        {
            // Return the world position without any zoom adjustment
            // The NodeEditor will handle the conversion between world and screen coordinates
            return node.Position + pin.Offset;
        }
    }
}
