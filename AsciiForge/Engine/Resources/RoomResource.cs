using System.Text.Json;
using System.Text.Json.Serialization;
using AsciiForge.Helpers.JsonConverters;

namespace AsciiForge.Engine.Resources
{
    public class RoomResource : Resource
    {
        private readonly InstanceResource[] _instances;
        public InstanceResource[] instances { get { return _instances; } }

        [JsonConstructor]
        private RoomResource(InstanceResource[] instances)
        {
            this._instances = instances;

            (bool isValid, string error) = IsValid();
            if (!isValid)
            {
                throw new ResourceFormatException(error);
            }
        }

        protected override (bool, string) IsValid()
        {
            bool isValid = false;
            string error = string.Empty;

            isValid = true;
            return (isValid, error);
        }
        
        public static async Task<RoomResource> Read(ResourceManager.ResourceFile resourceFile)
        {
            await using FileStream fileStream = File.Open(resourceFile.path, FileMode.Open, FileAccess.Read);
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
            };
            options.Converters.Add(new JsonComponentTypeConverter());
            options.Converters.Add(new JsonComponentPropertyConverter());
            RoomResource resource = await JsonSerializer.DeserializeAsync<RoomResource>(fileStream, options) ?? throw new ResourceFormatException($"Failed to deserialize room resource at: {resourceFile.path}");
            return resource;
        }
    }
}
