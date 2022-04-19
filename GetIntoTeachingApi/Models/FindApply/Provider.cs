using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
{
	public class Provider
	{
		[JsonProperty("name")]
		public string Name { get; set; }
	}
}

