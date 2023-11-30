using System.Text.Json;
using System.Text.Json.Serialization;
using AsciiForge.Helpers.JsonConverters;

namespace AsciiForge.Engine.Resources
{
    public class EntityResource : Resource
    {
        [JsonIgnore]
        public string species { get; internal set; }
        private readonly ComponentResource[] _components;
        public ComponentResource[] components { get { return _components; } }

        [JsonConstructor]
        private EntityResource(ComponentResource[] components)
        {
            species = string.Empty;
            _components = components;

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
        
        public static async Task<EntityResource> Read(ResourceManager.ResourceFile resourceFile)
        {
            await using FileStream fileStream = File.Open(resourceFile.path, FileMode.Open, FileAccess.Read);
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
            };
            options.Converters.Add(new JsonComponentTypeConverter());
            options.Converters.Add(new JsonComponentPropertyConverter());
            EntityResource resource = await JsonSerializer.DeserializeAsync<EntityResource>(fileStream, options) ?? throw new ResourceFormatException($"Failed to deserialize entity resource at: {resourceFile.path}");
            resource.species = resourceFile.name;
            return resource;
        }
    }
}
