using System.Text.Json;
using System.Text.Json.Serialization;

namespace AsciiForge.Helpers.JsonConverters
{
    internal class JsonExceptionConverter : JsonConverter<Exception>
    {
        public override Exception? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Message", value.Message);
            writer.WriteString("Source", value.Source);
            writer.WriteNumber("HResult", value.HResult);
            writer.WriteString("HelpLink", value.HelpLink);
            writer.WriteString("StackTrace", value.StackTrace);
            writer.WritePropertyName("InnerException");
            writer.WriteRawValue(JsonSerializer.Serialize(value.InnerException, options));
            writer.WriteEndObject();
        }
    }
}
