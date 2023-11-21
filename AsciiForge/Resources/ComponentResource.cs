using AsciiForge.Engine;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AsciiForge.Resources
{
    public class ComponentResource : Resource
    {
        private readonly Type _type;
        public Type type { get { return _type; } }
        private readonly (string, object?)[] _properties;
        public (string, object?)[] properties { get { return _properties; } }

        [JsonConstructor]
        public ComponentResource(Type type, (string, object?)[] properties)
        {
            this._type = type;
            this._properties = properties;

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

            if (_properties != null)
            {
                for (int i = 0; i < _properties.Length; i++)
                {
                    string key = _properties[i].Item1;
                    PropertyInfo? property = _type.GetProperty(key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (property == null)
                    {
                        Logger.Warning($"Referencing non-existing property '{key}' in component of type '{_type}'");
                        continue;
                    }
                    if (property.GetCustomAttribute(typeof(JsonIgnoreAttribute)) != null)
                    {
                        Logger.Warning($"Trying to set a JsonIgnore property '{key}' in component of type '{_type}'");
                        continue;
                    }
                    if (!property.CanWrite)
                    {
                        Logger.Warning($"Referencing property '{key}' which does not have a setter in component of type '{_type}'");
                        continue;
                    }
                    if (!property.CanRead)
                    {
                        Logger.Warning($"Referencing property '{key}' which does not have a getter in component of type '{_type}'");
                        continue;
                    }

                    try
                    {
                        JsonSerializerOptions options = new JsonSerializerOptions()
                        {
                            AllowTrailingCommas = true,
                            ReadCommentHandling = JsonCommentHandling.Skip,
                        };
                        options.Converters.Add(new JsonStringEnumConverter());
                        object? deserializedValue = JsonSerializer.Deserialize(_properties[i].Item2?.ToString() ?? string.Empty, property.PropertyType, options);
                        _properties[i].Item2 = deserializedValue;
                    }
                    catch (Exception)
                    {
                        error = $"Failed to parse value of property '{key}' to type '{property.PropertyType.Name}' in component of type '{_type}'";
                        return (isValid, error);
                    }
                }
            }

            isValid = true;
            return (isValid, error);
        }
    }
}
