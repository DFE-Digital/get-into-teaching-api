using System.Text.Json;
using Dahomey.Json;

namespace GetIntoTeachingApi.Utils
{
    public static class JsonExtensions
    {
        private static JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            IgnoreNullValues = true,
        }.SetupExtensions();

        public static T DeserializeChangedTracked<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }

        public static string SerializeChangedTracked<T>(this T value)
        {
            return JsonSerializer.Serialize(value, _options);
        }
    }
}
