using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
{
    public class Response<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
