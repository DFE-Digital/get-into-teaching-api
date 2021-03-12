using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GetIntoTeachingApi.Utils
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        public static T DeserializeChangeTracked<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        public static string SerializeChangeTracked<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, _settings);
        }
    }
}
