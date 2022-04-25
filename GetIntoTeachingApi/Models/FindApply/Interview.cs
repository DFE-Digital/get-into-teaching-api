using System;
using System.Globalization;
using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
{
	public class Interview
	{
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("date_and_time")]
		public DateTime DateAndTime { get; set; }
		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }
		[JsonProperty("updated_at")]
		public DateTime UpdatedAt { get; set; }
		[JsonProperty("cancelled_at")]
		public DateTime? CancelledAt { get; set; }

		public Crm.ApplicationInterview ToCrmModel()
		{
			return new Crm.ApplicationInterview()
			{
				FindApplyId = Id.ToString(CultureInfo.CurrentCulture),
				CreatedAt = CreatedAt,
				UpdatedAt = UpdatedAt,
				ScheduledAt = DateAndTime,
				CancelledAt = CancelledAt,
			};
		}
	}
}
