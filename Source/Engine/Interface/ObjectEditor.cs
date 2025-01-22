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

namespace Moonborne.Engine.UI
{
    public static class ObjectEditor
    {
        public static string WindowName = "Object Editor";

        public static void Draw()
        {
            ImGui.Begin(WindowName);

            if (RoomEditor.selectedLayer != null && RoomEditor.selectedLayer.Type == LayerType.Object)
            {

                // Display a list of all objects, and allow us to drag them into the game
                var list = ObjectLibrary.GetAllGameObjectNames();

                foreach (var name in list)
                {
                    // Draw the object image if possible
                    IntPtr tex = SpriteManager.GetImGuiTexture(name);
                    Texture2D realTex = SpriteManager.GetRawTexture(name);
                    float imageWidth = realTex.Width;
                    float imageHeight = realTex.Height;
                    imageWidth = Math.Clamp(imageWidth, 64, 64);
                    imageHeight = Math.Clamp(imageHeight, 64, 64);

                    if (tex == IntPtr.Zero)
                        tex = SpriteManager.GetImGuiTexture("QuestionMark");

                    ImGui.SameLine();

                    ImGui.PushStyleColor(ImGuiCol.ButtonActive,new System.Numerics.Vector4(0,0,0,0));
                    ImGui.PushStyleColor(ImGuiCol.Button,new System.Numerics.Vector4(0,0,0,0));
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered,new System.Numerics.Vector4(0,0,0,0));
                    System.Numerics.Vector2 pos = ImGui.GetCursorPos();
                    ImGui.PopStyleColor(3);

                    ImGui.ImageButton($"{name}", tex, new System.Numerics.Vector2(imageWidth, imageHeight));
                    ImGui.SameLine();
                    ImGui.SetCursorPos(pos);
                    pos = ImGui.GetCursorPos();
                    ImGui.SameLine();
                    ImGui.SetCursorPos(pos);

                    // Select the object we want to drag into the game
                    if (ImGui.BeginDragDropSource())
                    {
                        Vector2 position = InputManager.MouseWorldCoords();

                        if (!RoomEditor.Dragging)
                        {
                            var newObject = ObjectLibrary.CreateObject(name, position, RoomEditor.selectedLayer.Name);
                            RoomEditor.selectedObject = newObject;
                        }

                        position.X = ((int)position.X / RoomEditor.CellSize) * RoomEditor.CellSize;
                        position.Y = ((int)position.Y / RoomEditor.CellSize) * RoomEditor.CellSize;
                        RoomEditor.selectedObject.Transform.Position = position;

                        ImGui.Text($"Place: {name}"); // Visual feedback during dragging
                        RoomEditor.Dragging = true;
                        ImGui.EndDragDropSource();
                    }
                }

                // Drag object into world
                if (InputManager.MouseLeftReleased() && RoomEditor.Dragging)
                {
                    if (RoomEditor.HoveringOverGameWorld)
                    {
                        Vector2 position = InputManager.MouseWorldCoords();
                        Console.WriteLine($"Created {RoomEditor.selectedObject} at {position}");
                        Inspector.SelectedObject = RoomEditor.selectedObject;
                    }
                    else
                    {
                        LayerManager.RemoveInstance(RoomEditor.selectedObject);
                    }
                    RoomEditor.Dragging = false;
                }
            }
            

            ImGui.End();
        }
    }
}
