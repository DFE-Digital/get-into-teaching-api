using System;
using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
{
	public class Course
	{
		[JsonProperty("uuid")]
		public Guid? Id { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
	}
}
