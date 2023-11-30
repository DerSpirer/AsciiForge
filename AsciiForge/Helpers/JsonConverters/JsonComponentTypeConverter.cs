using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AsciiForge.Engine.Ecs;
using AsciiForge.Engine.IO;

namespace AsciiForge.Helpers.JsonConverters
{
    internal class JsonComponentTypeConverter : JsonConverter<Type>
    {
        private static Type[]? _componentTypes;
        private static Type[] componentTypes
        {
            get
            {
                if (_componentTypes == null)
                {
                    // Initialize component types
                    List<Type> componentTypes = new List<Type>();
                    Type component = typeof(Component);
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => !string.IsNullOrEmpty(a.FullName) &&
                                    !a.FullName.StartsWith("System") &&
                                    !a.FullName.StartsWith("Microsoft") &&
                                    !a.FullName.StartsWith("netstandard")).ToArray();
                    foreach (Assembly assembly in assemblies)
                    {
                        foreach (Type type in assembly.GetExportedTypes().Where(t => t.IsClass && component.IsAssignableFrom(t) && !t.IsAbstract))
                        {
                            if (type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new Type[0]) == null)
                            {
                                Logger.Warning($"Component type '{type.Name}' does not contain a definition of a public parameterless constructor. It will not be able to be instantiated.");
                                continue;
                            }
                            componentTypes.Add(type);
                        }
                    }
                    _componentTypes = componentTypes.ToArray();
                }
                return _componentTypes;
            }
        }

        public override Type? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Unexpected token type: {reader.TokenType}");
            }
            string? typeName = reader.GetString();
            Type? componentType = componentTypes.Where(c => c.FullName == typeName).FirstOrDefault();
            if (componentType == null)
            {
                throw new JsonException($"Unknown component type {typeName}");
            }
            return componentType;
        }

        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.AssemblyQualifiedName);
        }
    }
}
