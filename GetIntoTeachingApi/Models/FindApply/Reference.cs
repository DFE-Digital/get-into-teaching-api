using System;
using System.Globalization;
using GetIntoTeachingApi.Utils;
using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
{
	public class Reference
	{
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("requested_at")]
		public DateTime? RequestedAt { get; set; }
		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }
		[JsonProperty("updated_at")]
		public DateTime UpdatedAt { get; set; }
		[JsonProperty("feedback_status")]
		public string FeedbackStatus { get; set; }
		[JsonProperty("referee_type")]
		public string RefereeType { get; set; }

		public Crm.ApplicationReference ToCrmModel()
		{
			return new Crm.ApplicationReference()
			{
				FindApplyId = Id.ToString(CultureInfo.CurrentCulture),
				RequestedAt = RequestedAt,
				CreatedAt = CreatedAt,
				UpdatedAt = UpdatedAt,
				FeedbackStatusId = (int)Enum.Parse(typeof(Crm.ApplicationReference.FeedbackStatus), FeedbackStatus.ToPascalCase()),
				Type = RefereeType,
			};
		}
	}
}
