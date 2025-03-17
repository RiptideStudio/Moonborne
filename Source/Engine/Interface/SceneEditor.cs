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
using Moonborne.Game.Objects.Prefabs;
using Moonborne.Game.Gameplay;
using Microsoft.Xna.Framework.Input;

namespace Moonborne.Engine.UI
{
    public static class SceneEditor
    {
        public static string WindowName = "Scene Editor";
        public static bool SelectedLayer = false;
        public static Layer LayerToRemove;
        public static bool isSelected = false;

        /// <summary>
        /// Draw the inspector
        /// </summary>
        public static void Draw()
        {
            ImGui.Begin(WindowName);

            if (ImGui.IsItemClicked())
            {
                PrefabEditor.IsActive = false;
            }

            // Create new layers to the layer manager
            if (ImGui.BeginPopupContextItem("LayerContextMenu"))
            {
                // Add Object Layer
                if (ImGui.MenuItem("Create New Object Layer"))
                {
                    Layer newLayer = new Layer(1, () => Camera.TransformMatrix, LayerType.Object);
                    LayerManager.AddLayer(newLayer, RoomEditor.NewLayerName);
                    LayerManager.AddTilemap(new Tilemap("None", new int[100, 100], 16, RoomEditor.NewLayerName), RoomEditor.NewLayerName);
                    RoomEditor.SelectLayer(newLayer);
                }

                // Add Tile Layer
                if (ImGui.MenuItem("Create New Tile Layer"))
                {
                    Layer newLayer = new Layer(1, () => Camera.TransformMatrix, LayerType.Tile);
                    LayerManager.AddLayer(newLayer, RoomEditor.NewLayerName);
                    LayerManager.AddTilemap(new Tilemap("None", new int[100, 100], 16, RoomEditor.NewLayerName), RoomEditor.NewLayerName);
                    RoomEditor.SelectLayer(newLayer);
                }

                // Create object
                if (Inspector.SelectedLayer != null)
                {
                    if (ImGui.MenuItem("Create Object"))
                    {
                        EmptyObject obj = new EmptyObject();
                        LayerManager.AddInstance(obj, (Layer)Inspector.SelectedLayer);
                    }

                    // Delete layer
                    if (ImGui.MenuItem("Delete Layer"))
                    {
                        LayerManager.RemoveLayer(LayerToRemove);
                        RoomEditor.SelectLayer(null);
                    }
                }

                // Delete object
                if (Inspector.SelectedObject != null)
                {
                    if (ImGui.MenuItem("Delete Object"))
                    {
                        LayerManager.RemoveInstance((GameObject)Inspector.SelectedObject);
                    }
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

            // Display all currently active layers and select them
            ImGui.Text($"{RoomEditor.CurrentRoom.Name} Layers");
            ImGui.Separator();
            Inspector.Draw("Layer", Inspector.SelectedLayer);

            foreach (var layer in LayerManager.Layers)
            {
                // Can't edit locked layers
                if (layer.Value.Locked)
                    continue;

                // Draw a visibility icon to toggle the layers on and off
                IntPtr texOn = SpriteManager.GetImGuiTexture("VisibilityIconOn");
                IntPtr texOff = SpriteManager.GetImGuiTexture("VisibilityIconOff");
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

                // Visibility toggle
                if (ImGui.ImageButton(layer.Value.Name, tex, new System.Numerics.Vector2(16, 16)))
                {
                    layer.Value.Visible = !layer.Value.Visible;
                }

                // Make sure these are all on the same line
                ImGui.SameLine();
                ImGui.SetCursorPosY(currentY);
                ImGui.PopStyleColor(3);

                // Show the layer's icon
                IntPtr texIcon = SpriteManager.GetImGuiTexture("ObjectIcon");

                switch (layer.Value.Type)
                {
                    case LayerType.Object:
                        texIcon = SpriteManager.GetImGuiTexture("ObjectIcon");
                        break;

                    case LayerType.Tile:
                        texIcon = SpriteManager.GetImGuiTexture("TileIcon");
                        break;

                    default:
                        texIcon = SpriteManager.GetImGuiTexture("ObjectIcon");
                        break;
                }

                ImGui.Image(texIcon, new System.Numerics.Vector2(16, 16));
                ImGui.SameLine();
                ImGui.SetCursorPosY(currentY);

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

                // delete hotkey
                if (InputManager.KeyReleased(Keys.Delete))
                {
                    LayerManager.RemoveInstance((GameObject)Inspector.SelectedObject);
                }

                if (isNodeOpen) // Start layer tree
                {
                    // Show each object in this layer and click on it to inspect it
                    foreach (GameObject obj in layer.Value.Objects)
                    {
                        ImGui.PushID(obj.GetHashCode()); // Use the hash code as the unique ID

                        // Use selectable with selection state
                        if (ImGui.Selectable($"{obj.GetType().Name}", isSelected))
                        {
                            isSelected = (Inspector.SelectedObject == obj);
                            Inspector.SelectedObject = obj;
                        }
                        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                        {
                            Inspector.SelectedObject = obj;
                        }
                        ImGui.PopID();
                    }

                    ImGui.TreePop(); // End layer tree
                }
            }

            // Drag our selecting object around the world
            if (Inspector.SelectedObject != null && RoomEditor.CanDrag)
            {
                if (InputManager.MouseLeftDown())
                {
                    Vector2 mousePosition = InputManager.MouseWorldCoords();
                    mousePosition.X = ((int)(mousePosition.X+(RoomEditor.CellSize/2)) / RoomEditor.CellSize) * RoomEditor.CellSize;
                    mousePosition.Y = ((int)(mousePosition.Y+(RoomEditor.CellSize / 2)) / RoomEditor.CellSize) * RoomEditor.CellSize;

                    ImGui.BeginTooltip();
                    ImGui.SetTooltip($"{((GameObject)Inspector.SelectedObject).Name}");

                    ImGui.EndTooltip();
                    GameObject gameObject = (GameObject)Inspector.SelectedObject;
                    gameObject.Transform.Position = mousePosition;
                }
            }

            ImGui.End();
        }
    }
}
