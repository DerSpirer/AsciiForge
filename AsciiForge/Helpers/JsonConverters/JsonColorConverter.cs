using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AsciiForge.Helpers.JsonConverters
{
    internal class JsonColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument document = JsonDocument.ParseValue(ref reader);

            try
            {
                if (Enum.TryParse(document.RootElement.GetString(), out KnownColor color))
                {
                    return Color.FromKnownColor(color);
                }
            }
            catch (Exception)
            {
            }

            try
            {
                if (document.RootElement.TryGetProperty("color", out JsonElement colorProp) && Enum.TryParse(colorProp.GetString(), out KnownColor knownColor))
                {
                    int a = 255;
                    if (document.RootElement.TryGetProperty("a", out JsonElement aElement))
                    {
                        aElement.TryGetInt32(out a);
                    }
                    Color color = Color.FromKnownColor(knownColor);
                    return Color.FromArgb(a, color.R, color.G, color.B);
                }
            }
            catch (Exception)
            {
            }

            try
            {
                if (document.RootElement.TryGetProperty("r", out JsonElement rElement) && rElement.TryGetInt32(out int r) &&
                document.RootElement.TryGetProperty("g", out JsonElement gElement) && gElement.TryGetInt32(out int g) &&
                document.RootElement.TryGetProperty("b", out JsonElement bElement) && bElement.TryGetInt32(out int b))
                {
                    int a = 255;
                    if (document.RootElement.TryGetProperty("a", out JsonElement aElement))
                    {
                        aElement.TryGetInt32(out a);
                    }
                    return Color.FromArgb(a, r, g, b);
                }
            }
            catch (Exception)
            {
            }

            throw new JsonException("Failed to read color JSON");

        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            if (value.IsNamedColor)
            {
                writer.WriteStringValue(value.Name);
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteNumber("a", value.A);
                writer.WriteNumber("r", value.R);
                writer.WriteNumber("g", value.G);
                writer.WriteNumber("b", value.B);
                writer.WriteEndObject();
            }
        }
    }
}
