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

namespace Moonborne.Engine.UI
{
    public static class ObjectEditor
    {
        public static string WindowName = "Prefabs";
        public static GameObject newObject = null;
        public static bool PrefabSelectTypeWindowOpen = false;

        public static void Draw()
        {
            ImGui.Begin(WindowName);

            // Display a list of all objects, and allow us to drag them into the game
            var list = ObjectLibrary.GetAllGameObjectNames();

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && ImGui.IsWindowHovered())
            {
                ImGui.OpenPopup("PrefabContextMenu");
            }

            // Click out of the prefab window
            if (ImGui.IsWindowHovered())
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) || ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    PrefabSelectTypeWindowOpen = false;
                }
            }

            // Opt to create a new prefab
            if (ImGui.BeginPopupContextItem("PrefabContextMenu"))
            {
                // Create a prefab
                if (ImGui.MenuItem("Create Prefab"))
                {
                    PrefabSelectTypeWindowOpen = true;
                }

                // Delete a prefab
                if (ImGui.MenuItem("Delete Prefab"))
                {
                    PrefabEditor.DeletePrefab(PrefabEditor.SelectedPrefab);
                }
                ImGui.EndPopup();
            }

            // Show a window of game objects
            if (PrefabSelectTypeWindowOpen && ImGui.Begin("Create Game Object",ImGuiWindowFlags.NoDocking))
            {
                // Create a new prefab
                List<string> objectNames = ObjectLibrary.GetAllGameObjectNames();

                // Display each possible game object type defined in code
                foreach (string objectName in objectNames)
                {
                    if (ImGui.Selectable($"{objectName}"))
                    {
                        GameObject prefab = ObjectLibrary.CreateObject(objectName);
                        prefab.DisplayName = objectName+PrefabEditor.Prefabs.Count.ToString();
                        PrefabEditor.Prefabs.Add(prefab);
                        PrefabEditor.SelectedPrefab = prefab;
                        PrefabSelectTypeWindowOpen = false;
                        Console.WriteLine($"Created new prefab {prefab.DisplayName}");
                    }
                }

                ImGui.End();
            }

            // Display each prefab and select it
            foreach (GameObject prefab in PrefabEditor.Prefabs)
            {
                // Draw the prefab thumbnail image
                IntPtr img = SpriteManager.GetImGuiTexture("None");
                if (prefab.SpriteIndex.Texture.Data != null)
                {
                    img = prefab.SpriteIndex.Texture.Icon;
                }

                float buttonSize = 64.0f; // Size of each button
                float padding = 8.0f; // Spacing between buttons

                // Get available space in the window
                float windowWidth = ImGui.GetContentRegionAvail().X*4;
                float xCursor = ImGui.GetCursorPosX(); // Current X position

                if (xCursor + buttonSize + padding > windowWidth)
                {
                    ImGui.NewLine(); // Move to a new line if there's no space left
                }

                if (ImGui.ImageButton($"{prefab.DisplayName}{prefab.InstanceID}", img, new System.Numerics.Vector2(buttonSize, buttonSize)))
                {
                    PrefabEditor.SelectedPrefab = prefab;
                    PrefabEditor.IsActive = true;
                }

                if (ImGui.IsItemHovered())
                {
                    if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) || ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                    {
                        PrefabEditor.SelectedPrefab = prefab;
                    }
                }

                // Show the prefab name when hovered over
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.SetTooltip($"{prefab.DisplayName}");
                    ImGui.EndTooltip();
                }

                // Move to the next button but check if it should wrap
                xCursor = ImGui.GetCursorPosX();
                if (xCursor + buttonSize + padding < windowWidth)
                {
                    ImGui.SameLine();
                }

                if (RoomEditor.selectedLayer != null && RoomEditor.selectedLayer.Type == LayerType.Object)
                {
                    // Select the object we want to drag into the game
                    if (ImGui.BeginDragDropSource())
                    {
                        Vector2 position = InputManager.MouseWorldCoords();

                        if (!RoomEditor.Dragging)
                        {
                            newObject = ObjectLibrary.CreatePrefab(prefab.GetType().Name, prefab.DisplayName, position, RoomEditor.selectedLayer.Name);
                            RoomEditor.selectedObject = newObject;
                        }

                        position.X = ((int)position.X / RoomEditor.CellSize) * RoomEditor.CellSize;
                        position.Y = ((int)position.Y / RoomEditor.CellSize) * RoomEditor.CellSize;
                        RoomEditor.selectedObject.Transform.Position = position;

                        if (newObject != null)
                        {
                            ImGui.Text($"Place: {newObject.DisplayName}"); // Visual feedback during dragging
                        }

                        RoomEditor.Dragging = true;
                        ImGui.EndDragDropSource();
                    }
                }
            }

            // Drag object into world
            if (InputManager.MouseLeftReleased() && RoomEditor.Dragging)
            {
                if (RoomEditor.HoveringOverGameWorld)
                {
                    Vector2 position = InputManager.MouseWorldCoords();
                    Console.WriteLine($"Created {newObject.DisplayName} at {position}");
                    Inspector.SelectedObject = RoomEditor.selectedObject;
                }
                else
                {
                    LayerManager.RemoveInstance(RoomEditor.selectedObject);
                }
                RoomEditor.Dragging = false;
            }

            ImGui.End();
        }
    }
}
