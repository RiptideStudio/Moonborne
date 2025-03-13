using System;
using System.Collections.Generic;
using System.IO;
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
    public void SaveJson(string path = @"Content/World/WorldData.json")
    {
        try
        {
            Layers = LayerManager.Layers; // Capture all layers

            string json = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
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
    public void LoadJsonIntoWorld(string path = @"Content/World/WorldData.json")
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("Save file not found. Loading empty world.");
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
                }
            }

            Console.WriteLine($"Loaded world from {path} successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading world from {path}: " + ex);
        }
    }
}
