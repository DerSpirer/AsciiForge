using System.Text.Json;
using System.Text.Json.Serialization;

namespace AsciiForge.Helpers.JsonConverters
{
    internal class JsonComponentPropertyConverter : JsonConverter<(string, object?)>
    {
        public override (string, object?) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            
            if (document.RootElement.GetArrayLength() != 2)
            {
                throw new JsonException("Invalid component property tuple length");
            }

            string key = document.RootElement[0].GetString()!;
            object? value = document.RootElement[1].GetRawText();

            return (key, value);
        }

        public override void Write(Utf8JsonWriter writer, (string, object?) value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteStringValue(value.Item1);
            if (value.Item2 == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.Item2.ToString());
            }
            writer.WriteEndArray();
        }
    }
}
