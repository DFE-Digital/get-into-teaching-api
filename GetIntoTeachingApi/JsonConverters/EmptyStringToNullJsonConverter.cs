using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GetIntoTeachingApi.JsonConverters
{
    public class EmptyStringToNullJsonConverter : JsonConverter<string>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(string);
        }

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();

            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                value = null;
            }

            writer.WriteStringValue(value);
        }
    }
}