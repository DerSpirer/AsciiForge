﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace AsciiForge.Helpers
{
    internal class JsonRectangularArrayConverter<T> : JsonConverter<T[,]> where T : struct
    {
        public override T[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            int height = document.RootElement.GetArrayLength();
            int width = document.RootElement.EnumerateArray().First().GetArrayLength();
            T[,] array = new T[height, width];
            
            int i = 0;
            foreach (JsonElement row in document.RootElement.EnumerateArray())
            {
                int j = 0;
                foreach (JsonElement element in row.EnumerateArray())
                {
                    array[i, j] = JsonSerializer.Deserialize<T>(element.GetRawText(), options)!;
                    j++;
                }
                i++;
            }

            return array;
        }

        public override void Write(Utf8JsonWriter writer, T[,] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            for (int i = 0; i < value.GetLength(0); i++)
            {
                writer.WriteStartArray();
                for (int j = 0; j < value.GetLength(1); j++)
                {
                    writer.WriteRawValue(value[i, j].ToString() ?? string.Empty);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }
    }
}
