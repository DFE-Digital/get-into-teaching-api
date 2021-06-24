using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
{
    public class CandidateAttributes
    {
        [JsonProperty("email_address")]
        public string Email { get; set; }
    }
}
