using Newtonsoft.Json;

namespace GetIntoTeachingApi.Utils
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

        public static T DeserializeChangedTracked<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        public static string SerializeChangedTracked<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, _settings);
        }
    }
}
