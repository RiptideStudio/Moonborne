/*
 * Author: Callen Betts (RiptideDev) 
 * Breif: Defines an inspector to display information about objects
 */

using ImGuiNET;
using Moonborne.Game.Objects;
using System.Reflection;
using System;
using Moonborne.Engine.Collision;
using Moonborne.Game.Room;
using Moonborne.Input;
using Microsoft.Xna.Framework;
using Moonborne.Graphics.Camera;
using Moonborne.Graphics;

namespace Moonborne.Engine.UI
{
    public static class SceneEditor
    {
        public static string WindowName = "Scene Editor";
        public static bool SelectedLayer = false;
        public static Layer LayerToRemove;

        /// <summary>
        /// Draw the inspector
        /// </summary>
        public static void Draw()
        {
            ImGui.Begin(WindowName);


            /// Create new layers to the layer manager
            if (ImGui.BeginPopupContextItem("LayerContextMenu"))
            {
                // Add Object Layer
                if (ImGui.MenuItem("Create New Layer"))
                {
                    Layer newLayer = new Layer(1, () => Camera.Transform, LayerType.Object);
                    LayerManager.AddLayer(newLayer, RoomEditor.NewLayerName);
                    LayerManager.AddTilemap(new Tilemap("None", new int[100, 100], 16, RoomEditor.NewLayerName), RoomEditor.NewLayerName);
                    RoomEditor.SelectLayer(newLayer);
                }

                // Delete layer
                if (ImGui.MenuItem("Delete Layer"))
                {
                    LayerManager.RemoveLayer(LayerToRemove);
                    RoomEditor.SelectLayer(null);
                }

                // Input for layer name
                ImGui.InputText("Layer Name", ref RoomEditor.NewLayerName, 20);

                ImGui.EndPopup();
            }

            // Trigger the context menu on right-click
            if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup("LayerContextMenu");
            }

            /// Display all currently active layers and select them
            ImGui.Text($"{RoomEditor.CurrentRoom.Name} Layers");
            ImGui.Separator();
            Inspector.Draw("Layer", Inspector.SelectedLayer);

            foreach (var layer in LayerManager.Layers)
            {
                /// Can't edit locked layers
                if (layer.Value.Locked)
                    continue;

                // Draw a visibility icon to toggle the layers on and off
                IntPtr texOn = ImGuiManager.imGuiRenderer.BindTexture(SpriteManager.GetTexture("VisibilityIconOn"));
                IntPtr texOff = ImGuiManager.imGuiRenderer.BindTexture(SpriteManager.GetTexture("VisibilityIconOff"));
                ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4(0, 0, 0, 0)); // Transparent background
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new System.Numerics.Vector4(1, 1, 1, 0.2f)); // Slight hover effect
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new System.Numerics.Vector4(1, 1, 1, 0.2f));

                float currentY = ImGui.GetCursorPosY();
                ImGui.SetCursorPosY(currentY-1);

                IntPtr tex = texOn;

                if (!layer.Value.Visible)
                {
                    tex = texOff;
                }

                if (ImGui.ImageButton(layer.Value.Name, tex, new System.Numerics.Vector2(16, 16)))
                {
                    layer.Value.Visible = !layer.Value.Visible;
                }
                ImGui.SameLine();
                ImGui.SetCursorPosY(currentY);
                ImGui.PopStyleColor(3);

                // Draw each layer in the hierarchy and all of its objects that belong to it
                // In the future there may be other properties like an array of tilemaps
                ImGuiTreeNodeFlags nodeFlags = ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick;

                // Check if this is the currently selected layer and mark it as selected
                if (RoomEditor.selectedLayer == layer.Value)
                {
                    nodeFlags |= ImGuiTreeNodeFlags.Selected; // Highlight the selected node
                }

                // Render the tree node and handle expansion
                bool isNodeOpen = ImGui.TreeNodeEx(layer.Value.Name, nodeFlags);

                if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    LayerToRemove = layer.Value;
                }

                // Handle selection logic independently of expansion
                if (ImGui.IsItemClicked())
                {
                    RoomEditor.SelectLayer(layer.Value);
                }

                if (isNodeOpen) // Start layer tree
                {
                    // Show each object in this layer and click on it to inspect it
                    foreach (GameObject obj in layer.Value.Objects)
                    {
                        ImGui.PushID(obj.GetHashCode()); // Use the hash code as the unique ID
                        if (ImGui.Selectable($"{obj.GetType().Name}"))
                        {
                            Inspector.SelectedObject = obj;
                        }
                    }

                    ImGui.TreePop(); // End layer tree
                }
            }

            // Drag our selecting object in the world
            if (Inspector.SelectedObject != null && RoomEditor.CanDrag)
            {
                if (InputManager.MouseLeftDown())
                {
                    Vector2 mousePosition = InputManager.MouseWorldCoords();
                    mousePosition.X = ((int)mousePosition.X / RoomEditor.CellSize) * RoomEditor.CellSize;
                    mousePosition.Y = ((int)mousePosition.Y / RoomEditor.CellSize) * RoomEditor.CellSize;

                    GameObject gameObject = (GameObject)Inspector.SelectedObject;
                    gameObject.Position = mousePosition;
                }
            }

            ImGui.End();
        }
    }
}
