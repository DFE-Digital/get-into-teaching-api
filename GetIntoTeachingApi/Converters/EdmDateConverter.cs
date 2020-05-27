using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OData.Edm;

namespace GetIntoTeachingApi.Converters
{
    public class EdmDateConverter : JsonConverter<Date>
    {
        public override Date Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Date.Parse(reader.GetString());

        public override void Write(Utf8JsonWriter writer, Date value, JsonSerializerOptions options) 
            => writer.WriteStringValue($"{value.Year}-{value.Month}-${value.Day}");
    }
}
