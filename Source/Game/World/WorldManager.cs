using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Moonborne.Engine.Components;
using Moonborne.Game.Assets;
using Moonborne.Game.Objects;
using Moonborne.Game.Room;
using Newtonsoft.Json;

public class WorldState
{
    public Dictionary<string, Layer> Layers = new Dictionary<string, Layer>();

    /// <summary>
    /// Save the world
    /// </summary>
    /// <param name="path"></param>
    public void SaveJson()
    {
        try
        {
            Layers = LayerManager.Layers; // Capture all layers

            string json = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            string path = $@"Content/World/{RoomEditor.CurrentRoom.Name}.json";
            File.WriteAllText(path, json);
            Console.WriteLine("Game world saved successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving world: " + ex);
        }
    }
    
    /// <summary>
    /// Load the world again
    /// </summary>
    /// <param name="path"></param>
    public void LoadJsonIntoWorld(string worldName, string path = @"Content/World/")
    {
        path += worldName + ".json";

        if (!File.Exists(path))
        {
            Console.WriteLine("Save file not found.");
            return;
        }
        try
        {
            // Load saved data
            WorldState loadedWorld = JsonConvert.DeserializeObject<WorldState>(File.ReadAllText(path), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });


            // Clear existing layers
            LayerManager.Layers.Clear();

            // Restore layers and objects
            foreach (var kvp in loadedWorld.Layers)
            {
                LayerManager.Layers[kvp.Key] = kvp.Value;
            }

            // Add back the objects to the global list with the layer they're on
            foreach (var kvp in loadedWorld.Layers)
            {
                Layer layer = kvp.Value;

                foreach (GameObject obj in layer.Objects)
                {
                    obj.Layer = layer;
                    LayerManager.Objects.Add(obj);

                    foreach (ObjectComponent comp in obj.Components)
                    {
                        comp.Parent = obj;
                        ResolveAssetReferences(comp);
                    }
                }
            }

            Console.WriteLine($"Loaded world from {path} successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading world from {path}: " + ex);
        }
    }

    /// <summary>
    /// Update all asset references saved in objects
    /// </summary>
    /// <param name="obj"></param>
    private static void ResolveAssetReferences(object obj)
    {
        if (obj == null) return;

        Type objType = obj.GetType();
        foreach (var field in objType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            object fieldValue = field.GetValue(obj);

            if (fieldValue is Asset assetValue)
            {
                // Replace outdated asset with the latest one from the registry
                Asset updatedAsset = AssetManager.GetAsset(assetValue.Folder, assetValue.Name);
                if (updatedAsset != null)
                {
                    field.SetValue(obj, updatedAsset);
                }
            }
            else if (fieldValue is IList list)
            {
                // Loop through lists and update any asset references inside
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] is Asset assetItem)
                    {
                        Asset updatedAsset = AssetManager.GetAsset(assetItem.Folder, assetItem.Name);
                        if (updatedAsset != null)
                        {
                            list[i] = updatedAsset;
                        }
                    }
                }
            }
            else if (fieldValue is IDictionary dictionary)
            {
                // Loop through dictionaries and update any asset values
                foreach (var key in dictionary.Keys)
                {
                    if (dictionary[key] is Asset assetItem)
                    {
                        Asset updatedAsset = AssetManager.GetAsset(assetItem.Folder, assetItem.Name);
                        if (updatedAsset != null)
                        {
                            dictionary[key] = updatedAsset;
                        }
                    }
                }
            }
            else if (fieldValue != null && fieldValue.GetType().IsClass)
            {
                // Recursively process nested objects
                ResolveAssetReferences(fieldValue);
            }
        }
    }

}
