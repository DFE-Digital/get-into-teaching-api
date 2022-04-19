using System;
using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
{
	public class Reference
	{
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("requested_at")]
		public DateTime RequestedAt { get; set; }
		[JsonProperty("feedback_status")]
		public string FeedbackStatus { get; set; }
		[JsonProperty("referee_type")]
		public string RefereeType { get; set; }
	}
}
