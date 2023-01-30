using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.Apply
{
    public class Response<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
