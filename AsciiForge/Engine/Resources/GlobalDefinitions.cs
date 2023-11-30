using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using AsciiForge.Engine.IO;

namespace AsciiForge.Engine.Resources;

public static class GlobalDefinitions
{
    [Required]
    public static string title { get; private set; }
    [Required]
    public static string[] roomsOrder { get; private set; }

    internal static async Task Load()
    {
        // Read and deserialize global definitions
        const string fileName = "globalDefinitions.json";
        JsonObject? defsObject = null;
        try
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            await using FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
            JsonDocumentOptions documentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip,
            };
            defsObject = (JsonObject)(await JsonNode.ParseAsync(fileStream, documentOptions: documentOptions))!;
        }
        catch (FileNotFoundException exception)
        {
            Logger.Critical($"File {fileName} not found", exception);
        }
        catch (Exception exception)
        {
            Logger.Critical($"Failed to deserialize {fileName}", exception);
        }
        if (defsObject == null)
        {
            throw new Exception($"Failed to deserialize {fileName}");
        }
        
        // Fill values of properties
        foreach (PropertyInfo property in typeof(GlobalDefinitions).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            bool isRequired = property.GetCustomAttribute<RequiredAttribute>() != null;
            if (!defsObject.TryGetPropertyValue(property.Name, out JsonNode? valueNode))
            {
                if (isRequired)
                {
                    Logger.Critical($"Missing required global definition '{property.Name}'");
                    throw new Exception($"Missing required global definition '{property.Name}'");
                }
                else
                {
                    continue;
                }
            }
            try
            {
                object? value = valueNode!.Deserialize(property.PropertyType);
                if (value == null && isRequired)
                {
                    Logger.Critical($"Global definition '{property.Name}' is null");
                    throw new Exception($"Global definition '{property.Name}' is null");
                }
                property.SetValue(null, value);
            }
            catch (Exception exception)
            {
                Logger.Error($"Failed to set global definition '{property.Name}'");
                if (isRequired)
                {
                    Logger.Critical($"Failed to set required global definition '{property.Name}'");
                    throw new Exception($"Failed to set required global definition '{property.Name}'");
                }
            }
        }
    }
    internal static List<RoomResource> OrderRooms(Dictionary<string, RoomResource> rooms)
    {
        List<RoomResource> ordered = new List<RoomResource>();
        foreach (string room in roomsOrder)
        {
            if (rooms.TryGetValue(room, out RoomResource resource))
            {
                ordered.Add(resource);
            }
            else
            {
                Logger.Warning($"Global definition 'roomsOrder' references non-existing room resource '{room}'");
            }
        }
        return ordered;
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    private class RequiredAttribute : Attribute
    {
    }
}