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
                    LayerManager.AddLayer(new Layer(1, () => Camera.Transform, LayerType.Object), RoomEditor.NewLayerName);
                    LayerManager.AddTilemap(new Tilemap("None", new int[100, 100], 16, RoomEditor.NewLayerName), RoomEditor.NewLayerName);
                }

                // Delete layer
                if (ImGui.MenuItem("Delete Layer"))
                {
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
            ImGui.Text("Layers");
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

                if (ImGui.ImageButton(layer.Value.Name, tex, new System.Numerics.Vector2(14, 8)))
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

                // Handle selection logic independently of expansion
                if (ImGui.IsItemClicked())
                {
                    RoomEditor.selectedLayer = layer.Value;
                    Inspector.SelectedLayer = RoomEditor.selectedLayer;
                    // Select different tile layers
                    if (layer.Value.Type == LayerType.Tile)
                    {
                        RoomEditor.SelectedTilemap = layer.Value.Tilemaps[0];
                    }
                    else if (layer.Value.Type == LayerType.Object)
                    {
                        RoomEditor.SelectedTilemap = null;
                    }
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

            ImGui.End();
        }
    }
}
