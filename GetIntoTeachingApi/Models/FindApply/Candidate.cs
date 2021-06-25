using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
{
    public class Candidate
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("attributes")]
        public CandidateAttributes Attributes { get; set; }
    }
}
