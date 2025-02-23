/*
 * Author: Callen Betts (RiptideDev) 
 * Breif: Defines an inspector to display information about objects
 */

using ImGuiNET;
using Moonborne.Game.Objects;
using System.Reflection;
using System;
using Moonborne.Input;
using Microsoft.Xna.Framework.Input;
using Moonborne.Game.Room;
using Microsoft.Xna.Framework;
using Moonborne.Engine.Components;
using System.Linq;
using System.Collections.Generic;
using Moonborne.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Moonborne.UI.Dialogue;

namespace Moonborne.Engine.UI
{
    public static class Inspector
    {
        public static bool Visible;
        public static object SelectedObject;
        public static object SelectedLayer;
        public static string WindowName = "Inspector";
        public static string SelectedKey = "";
        public static string SelectedItemTitle = "Inspector";
        private static object cachedObj;

        /// <summary>
        /// Deletes the currently selected object
        /// </summary>
        public static void DeleteSelectedObject()
        {
            if (SelectedObject != null)
            {
                LayerManager.RemoveInstance((GameObject)SelectedObject);
                SelectedObject = null;
            }
        }

        /// <summary>
        /// Draw the inspector
        /// </summary>
        public static void Draw(string name="Inspector", object obj = null)
        {
            ImGui.Begin(name);

            // Display properties of the selected object
            if (obj != null)
            {
                ImGui.Text("Editing "+obj.GetType().Name);
                ImGui.Separator();

                Type objectType = obj.GetType();
                List<PropertyInfo> properties = objectType.GetProperties().ToList();

                if (cachedObj == null || cachedObj.GetType() != obj.GetType())
                    cachedObj = obj;

                // Get all component properties 
                if (obj as GameObject != null)
                {
                    GameObject gameObject = (GameObject)cachedObj;

                    foreach (ObjectComponent component in gameObject.Components)
                    {
                        if (ImGui.TreeNodeEx(component.Name))
                        {
                            Type componentType = component.GetType();
                            PropertyInfo[] componentProperties = componentType.GetProperties();

                            foreach (var property in componentProperties)
                            {
                                DrawPropertyInEditor(property, component);
                            }
                            ImGui.TreePop();
                        }
                    }
                }

                // Attempt to delete the selected object
                if (InputManager.KeyTriggered(Keys.Delete))
                {
                    DeleteSelectedObject();
                }

                // Draw object base properties
                if (ImGui.TreeNodeEx($"{cachedObj.GetType().Name} (Self)"))
                {
                    foreach (var property in properties)
                    {
                        DrawPropertyInEditor(property, cachedObj);
                    }
                    ImGui.TreePop();
                }
            }

            ImGui.End();
        }

        public static void DrawPropertyInEditor(PropertyInfo property, object obj)
        {
            // Only display editable properties (must be readable and writable)
            if (property.CanRead && property.CanWrite)
            {
                object value = property.GetValue(obj);

                // Render appropriate ImGui input widget based on the property's type
                if (value is int intValue)
                {
                    int newValue = intValue;
                    if (ImGui.InputInt(property.Name, ref newValue))
                    {
                        property.SetValue(obj, newValue);
                    }
                }
                else if (value is float floatValue)
                {
                    float newValue = floatValue;
                    if (ImGui.InputFloat(property.Name, ref newValue))
                    {
                        property.SetValue(obj, newValue);
                    }
                }
                else if (value is bool boolValue)
                {
                    bool newValue = boolValue;
                    if (ImGui.Checkbox(property.Name, ref newValue))
                    {
                        property.SetValue(obj, newValue);
                    }
                }
                else if (value is string stringValue)
                {
                    // Create a local copy of the string to work with
                    string newValue = stringValue;

                    // InputText modifies newValue directly
                    if (ImGui.InputText(property.Name, ref newValue, 256))
                    {
                        property.SetValue(obj, newValue);                 
                    }
                }
                else if (value is Enum enumValue)
                {
                    string[] enumNames = Enum.GetNames(property.PropertyType);
                    int selectedIndex = Array.IndexOf(enumNames, value.ToString());
                    if (ImGui.Combo(property.Name, ref selectedIndex, enumNames, enumNames.Length))
                    {
                        property.SetValue(obj, Enum.Parse(property.PropertyType, enumNames[selectedIndex]));
                    }
                }
                else if (value is Vector2 vector)
                {
                    float xValue = vector.X;
                    float yValue = vector.Y;
                    Vector2 targetVal = new Vector2(xValue, yValue);
                    if (ImGui.InputFloat(property.Name +" X", ref xValue))
                    {
                        targetVal.X = xValue;
                    }
                    if (ImGui.InputFloat(property.Name+" Y", ref yValue))
                    {
                        targetVal.Y = yValue;
                    }
                    property.SetValue(obj, targetVal);
                }
                else if (value is SpriteTexture)
                {
                    // Draw the dropdown menu
                    if (ImGui.CollapsingHeader("Texture"))
                    {
                        // Display a dropdown of all textures to swap to
                        foreach (var sprite in SpriteManager.sprites)
                        {
                            SpriteTexture spr = sprite.Value;
                            SpriteTexture tex = (SpriteTexture)value;

                            // Select the texture
                            if (ImGui.Selectable(sprite.Key))
                            {
                                property.SetValue(obj, SpriteManager.GetTexture(sprite.Key));
                                Console.WriteLine($"Texture is now {sprite.Key}");
                            }
                        }
                    }
                }
                else if (value is List<Dialogue> dialogueList)
                {
                    NPC npc = ((NPC)obj);

                    if (ImGui.CollapsingHeader("Dialogue"))
                    {
                        // Dropdown for selecting a dialogue to add
                        List<string> dialogueKeys = DialogueManager.Dialogue.Keys.ToList();

                        if (ImGui.BeginCombo("Add Dialogue", SelectedKey))
                        {
                            foreach (var key in dialogueKeys)
                            {
                                bool isSelected = (SelectedKey == key);
                                if (ImGui.Selectable(key, isSelected))
                                {
                                    SelectedKey = key;
                                    npc.DialogueObjects.Add(DialogueManager.Dialogue[SelectedKey]);
                                }

                                if (isSelected)
                                    ImGui.SetItemDefaultFocus();
                            }
                            ImGui.EndCombo();
                        }

                        // Display the list of assigned dialogues
                        for (int i = 0; i < npc.DialogueObjects.Count; i++)
                        {
                            Dialogue dialogue = npc.DialogueObjects[i];

                            ImGui.Text($"{i + 1}. {dialogue.Name}");

                            // Remove button
                            ImGui.SameLine();
                            if (ImGui.Button($"Remove##{i}"))
                            {
                                npc.DialogueObjects.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }

            }
        }
    }
}
