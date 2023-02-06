using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.Apply
{
	public class Provider
	{
		[JsonProperty("name")]
		public string Name { get; set; }
	}
}
