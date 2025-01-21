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

namespace Moonborne.Engine.UI
{
    public static class Inspector
    {
        public static bool Visible;
        public static object SelectedObject;
        public static object SelectedLayer;
        public static string WindowName = "Inspector";
        public static string SelectedItemTitle = "Inspector";

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
                PropertyInfo[] properties = objectType.GetProperties();

                // Attempt to delete the selected object
                if (InputManager.KeyTriggered(Keys.Delete))
                {
                    DeleteSelectedObject();
                }

                foreach (var property in properties)
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
                            string newValue = stringValue;
                            byte[] buffer = new byte[256];
                            Array.Copy(System.Text.Encoding.UTF8.GetBytes(stringValue), buffer, stringValue.Length);

                            if (ImGui.InputText(property.Name, buffer, (uint)buffer.Length))
                            {
                                newValue = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
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
                    }
                }
            }

            ImGui.End();
        }
    }
}
