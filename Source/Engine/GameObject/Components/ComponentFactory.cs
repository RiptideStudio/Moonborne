using System.Collections.Generic;
using System;
using Moonborne.Engine.Components;
using Moonborne.Graphics;
using Newtonsoft.Json;
using Moonborne.Game.Components;
using Moonborne.Game.Objects;
using Moonborne.Engine.Graphics.Lighting;

public static class ComponentRegistry
{
    private static List<Type> componentTypes = new List<Type>();

    static ComponentRegistry()
    {
        RegisterComponent(typeof(Light));
        RegisterComponent(typeof(NPCBehavior));
        RegisterComponent(typeof(CameraFollow));
        RegisterComponent(typeof(PlayerBehavior));
        RegisterComponent(typeof(Transform));
        RegisterComponent(typeof(Sprite));
        RegisterComponent(typeof(Physics));
    }

    /// <summary>
    /// Registers a new component type for the editor.
    /// </summary>
    public static void RegisterComponent(Type componentType)
    {
        if (typeof(ObjectComponent).IsAssignableFrom(componentType) && !componentTypes.Contains(componentType))
        {
            componentTypes.Add(componentType);
        }
    }

    /// <summary>
    /// Returns all registered component types.
    /// </summary>
    public static List<Type> GetAllComponentTypes()
    {
        return componentTypes;
    }
}
public class ObjectComponentConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(ObjectComponent).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        // Load JSON object
        var jsonObject = Newtonsoft.Json.Linq.JObject.Load(reader);

        // Extract the type name from the JSON
        string typeName = jsonObject["$type"]?.ToString();
        if (string.IsNullOrEmpty(typeName))
            throw new JsonSerializationException("Missing $type field for component deserialization.");

        // Find the actual component type
        Type type = Type.GetType(typeName);
        if (type == null || !typeof(ObjectComponent).IsAssignableFrom(type))
            throw new JsonSerializationException($"Unknown component type: {typeName}");

        // Deserialize JSON into the correct component type
        return JsonConvert.DeserializeObject(jsonObject.ToString(), type);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var jsonObject = Newtonsoft.Json.Linq.JObject.FromObject(value);
        jsonObject.AddFirst(new Newtonsoft.Json.Linq.JProperty("$type", value.GetType().AssemblyQualifiedName));
        jsonObject.WriteTo(writer);
    }
}