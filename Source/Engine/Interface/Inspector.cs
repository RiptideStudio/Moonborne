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
using System.Collections;

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

        private static void RenderFields(object obj)
        {
            if (obj == null) return;
            Type type = obj.GetType();
            ImGui.Separator();
            ImGui.NewLine();

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                object value = field.GetValue(obj);
                RenderField(obj, field, value);
            }

            ImGui.NewLine();
            ImGui.Separator();
        }

        private static void RenderField(object obj, FieldInfo field, object value)
        {
            string name = field.Name;

            if (value == null)
            {
                ImGui.Text($"{name}: NULL");
                return;
            }

            Type valueType = value.GetType();

            // Primitive Types
            if (valueType.IsPrimitive || value is decimal || value is string)
            {
                if (value is int intValue) { if (ImGui.DragInt(name, ref intValue)) field.SetValue(obj, intValue); }
                else if (value is float floatValue) { if (ImGui.DragFloat(name, ref floatValue, 0.1f)) field.SetValue(obj, floatValue); }
                else if (value is double doubleValue) { float f = (float)doubleValue; if (ImGui.DragFloat(name, ref f)) field.SetValue(obj, (double)f); }
                else if (value is bool boolValue) { if (ImGui.Checkbox(name, ref boolValue)) field.SetValue(obj, boolValue); }
                else if (value is string strValue)
                {
                    // Convert string to a mutable buffer
                    byte[] buffer = new byte[256];
                    byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(strValue);
                    Array.Copy(strBytes, buffer, Math.Min(strBytes.Length, buffer.Length - 1));

                    // Editable text field
                    if (ImGui.InputText(name, buffer, (uint)buffer.Length))
                    {
                        // Convert back to a C# string
                        string newStr = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                        field.SetValue(obj, newStr);
                    }
                }
                return;
            }

            // Vector Types
            if (value is Vector2 xnaVector2)
            {
                System.Numerics.Vector2 temp = new System.Numerics.Vector2(xnaVector2.X, xnaVector2.Y);
                if (ImGui.DragFloat2(name, ref temp))
                {
                    field.SetValue(obj, new Vector2(temp.X, temp.Y));
                }
                return;
            }
            // Lists & Arrays
            if (typeof(IList).IsAssignableFrom(valueType))
            {
                IList list = (IList)value;

                // Check if it's a generic list
                Type listType = valueType.IsGenericType ? valueType.GetGenericArguments()[0] : typeof(object);

                if (ImGui.TreeNode($"{name} (List of {listType.Name})"))
                {
                    // Display all list elements
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (ImGui.TreeNode($"{name}[{i}]"))
                        {
                            RenderField(obj, field, list[i]); // Recursively render list elements

                            // Remove Button
                            if (ImGui.Button($"Remove##{name}{i}"))
                            {
                                list.RemoveAt(i);
                                ImGui.TreePop();
                                break; // Avoid modifying list while iterating
                            }

                            ImGui.TreePop();
                        }
                    }

                    // Add New Element Button
                    if (ImGui.Button($"Add New {listType.Name}##{name}"))
                    {
                        object newInstance = Activator.CreateInstance(listType);
                        list.Add(newInstance);
                    }

                    ImGui.TreePop();
                }
                return;
            }


            // Custom Objects (Recurse)
            if (valueType.IsClass)
            {
                if (ImGui.TreeNode(name))
                {
                    RenderFields(value);
                    ImGui.TreePop();
                }
            }
        }

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
            if (obj == null)
                return;

            ImGui.Begin(name);
            ImGui.Text($"Editing {obj.GetType().Name}");

            RenderFields(obj);

            ImGui.End();
        }
    }
}
