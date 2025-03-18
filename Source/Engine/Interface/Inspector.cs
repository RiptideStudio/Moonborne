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
using Moonborne.Game.Objects.Prefabs;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Xml.Linq;
using Moonborne.Game.Assets;

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

        private static void RenderFields(object obj)
        {
            if (obj == null) return;

            Type type = obj.GetType();
            ImGui.Separator();
            ImGui.NewLine();

            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                object value = null;
                MemberInfo memberInfo = member;

                if (member is FieldInfo field)
                {
                    value = field.GetValue(obj);
                }

                RenderValue(obj, memberInfo, value);
            }

            ImGui.NewLine();
            ImGui.Separator();
        }

        private static void SetValue(object obj, MemberInfo member, object newValue)
        {
            if (member is FieldInfo field)
            {
                field.SetValue(obj, newValue);
            }
            else if (member is PropertyInfo property)
            {
                if (property.CanWrite)
                    property.SetValue(obj, newValue);
            }
        }

        private static void RenderValue(object obj, MemberInfo member, object value)
        {
            string name = member.Name;

            if (member is FieldInfo field)
            {
                if (field.FieldType == typeof(SpriteTexture))
                {
                    RenderTextureField(obj, member, ((Sprite)(obj)).Texture);
                    return;
                }
            }

            if (value == null)
            {
                return;
            }

            Type valueType = value.GetType();

            // Primitive Types
            if (valueType.IsPrimitive || value is decimal || value is string)
            {
                if (value is int intValue) { if (ImGui.InputInt(name, ref intValue)) SetValue(obj, member, intValue); }
                else if (value is float floatValue) { if (ImGui.InputFloat(name, ref floatValue, 0.1f)) SetValue(obj, member, floatValue); }
                else if (value is bool boolValue) { if (ImGui.Checkbox(name, ref boolValue)) SetValue(obj, member, boolValue); }
                else if (value is string strValue)
                {
                    byte[] buffer = new byte[256];
                    byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(strValue);
                    Array.Copy(strBytes, buffer, Math.Min(strBytes.Length, buffer.Length - 1));

                    if (ImGui.InputText(name, buffer, (uint)buffer.Length))
                    {
                        // Convert bytes to string properly
                        string newStr = System.Text.Encoding.UTF8.GetString(buffer).Split('\0')[0];

                        // Ensure null or empty string is handled safely
                        newStr = string.IsNullOrWhiteSpace(newStr) ? "" : newStr;

                        SetValue(obj, member, newStr);
                    }
                }

                return;
            }

            // Vector Types
            if (value is Vector2 xnaVector2)
            {
                System.Numerics.Vector2 temp = new System.Numerics.Vector2(xnaVector2.X, xnaVector2.Y);
                if (ImGui.InputFloat2(name, ref temp))
                {
                    SetValue(obj, member, new Vector2(temp.X, temp.Y));
                }
                return;
            }

            // Handle Lists
            if (typeof(IList).IsAssignableFrom(valueType))
            {
                IList list = (IList)value;
                Type listType = valueType.IsGenericType ? valueType.GetGenericArguments()[0] : typeof(object);

                if (ImGui.TreeNode($"{name} (List of {listType.Name})"))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (ImGui.TreeNode($"{name}[{i}]"))
                        {
                            RenderFields(list[i]);

                            if (ImGui.Button($"Remove##{name}{i}"))
                            {
                                list.RemoveAt(i);
                                ImGui.TreePop();
                                break;
                            }

                            ImGui.TreePop();
                        }
                    }

                    if (ImGui.Button($"Add New {listType.Name}##{name}"))
                    {
                        object newInstance;

                        if (listType == typeof(string))
                            newInstance = string.Empty;
                        else if (listType == typeof(int))
                            newInstance = 0;
                        else if (listType == typeof(float))
                            newInstance = 0f;
                        else if (listType == typeof(bool))
                            newInstance = false;
                        else
                            newInstance = Activator.CreateInstance(listType);

                        list.Add(newInstance);
                    }

                    ImGui.TreePop();
                }
                return;
            }


            // Nested classes
            if (valueType.IsClass && valueType != typeof(string))
            {
                if (ImGui.TreeNode(name)) // Expandable section for nested objects
                {
                    RenderFields(value); // Recursively render fields of the nested object
                    ImGui.TreePop();
                }
                return;
            }

        }

        private static void RenderTextureField(object obj, MemberInfo member, SpriteTexture currentTexture)
        {
            // Button to open texture selection popup
            if (ImGui.Button($"Select Texture##{member.Name}"))
            {
                ImGui.OpenPopup("TextureSelector");
            }

            // Show texture selection menu
            if (ImGui.BeginPopup("TextureSelector"))
            {
                foreach (var tex in SpriteManager.textures) // Replace with your texture manager
                {
                    if (ImGui.Selectable(tex.Key))
                    {
                        SpriteTexture newSpriteTexture = SpriteManager.GetTexture(tex.Key);

                        SetValue(obj, member, newSpriteTexture);
                    }
                }
                ImGui.EndPopup();
            }

            // Display a preview image (if available)
            if (currentTexture != null)
            {
                ImGui.Image(SpriteManager.GetImGuiTexture(currentTexture.Name), new System.Numerics.Vector2(64, 64));
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
        public static void Draw(string name = "Inspector", object obj = null)
        {
            if (obj == null)
                return;

            ImGui.Begin(name);

            // Show the display name of the asset
            if (obj is Asset asset)
            {
                ImGui.Text($"Editing {asset.Name}");
            }
            else
            {
                ImGui.Text($"Editing {obj.GetType().Name}");
            }

            // Draw the object's public fields
            RenderFields(obj);

            // Draw the components
            if (obj is IComponentContainer holder)
            {
                // Draw component
                foreach (ObjectComponent component in holder.Components)
                {
                    if (ImGui.TreeNodeEx($"{component.Name}"))
                    {
                        RenderFields(component);
                        ImGui.TreePop();
                    }
                }

                // Button to add a new component
                if (ImGui.Button("Add Component"))
                {
                    ImGui.OpenPopup("AddComponentPopup");
                }

                // Show popup for adding components
                if (ImGui.BeginPopup("AddComponentPopup"))
                {
                    foreach (var componentType in ComponentRegistry.GetAllComponentTypes())
                    {
                        if (ImGui.MenuItem(componentType.Name))
                        {
                            holder.AddComponent(Activator.CreateInstance(componentType) as ObjectComponent);
                        }
                    }
                    ImGui.EndPopup();
                }
            }

            ImGui.End();
        }
    }
}
