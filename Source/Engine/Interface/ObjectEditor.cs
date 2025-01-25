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
        public static string WindowName = "Resources";
        public static GameObject newObject = null;

        public static void Draw()
        {
            ImGui.Begin(WindowName);

            // Display a list of all objects, and allow us to drag them into the game
            var list = ObjectLibrary.GetAllGameObjectNames();

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && ImGui.IsWindowHovered())
            {
                ImGui.OpenPopup("PrefabContextMenu");
            }

            // Opt to create a new prefab
            if (ImGui.BeginPopupContextItem("PrefabContextMenu"))
            {
                // Create a new prefab
                List<string> objectNames = ObjectLibrary.GetAllGameObjectNames();

                if (ImGui.CollapsingHeader("Create Prefab"))
                {
                    // Display each possible game object type defined in code
                    foreach (string objectName in objectNames)
                    {
                        if (ImGui.Selectable($"{objectName}"))
                        {
                            GameObject prefab = ObjectLibrary.CreateObject(objectName);
                            prefab.DisplayName = objectName;
                            PrefabEditor.Prefabs.Add(prefab);
                            PrefabEditor.SelectedPrefab = prefab;
                        }
                    }
                }                        
                // Delete a prefab
                if (ImGui.MenuItem("Delete Prefab"))
                {
                    PrefabEditor.DeletePrefab(PrefabEditor.SelectedPrefab);
                }
                ImGui.EndPopup();
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

                if (ImGui.ImageButton($"{prefab.DisplayName}{prefab.InstanceID}", img, new System.Numerics.Vector2(64,64)))
                {
                    PrefabEditor.SelectedPrefab = prefab;
                    PrefabEditor.IsActive = true;
                }
                // Show the prefab name when hovered over
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.SetTooltip($"{prefab.DisplayName}");
                    ImGui.EndTooltip();
                }
                // Keep horizontally alligned
                ImGui.SameLine();

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
