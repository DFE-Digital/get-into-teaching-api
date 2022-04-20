using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GetIntoTeachingApi.Utils;
using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
{
    public class ApplicationChoice
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("provider")]
        public Provider Provider { get; set; }
        [JsonProperty("course")]
        public Course Course { get; set; }
        [JsonProperty("interviews")]
        public IEnumerable<Interview> Interviews { get; set; }

        public Crm.ApplicationChoice ToCrmModel()
        {
            return new Crm.ApplicationChoice()
            {
                FindApplyId = Id.ToString(CultureInfo.CurrentCulture),
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
                StatusId = (int)Enum.Parse(typeof(Crm.ApplicationChoice.Status), Status.ToPascalCase()),
                CourseId = Course.Id.ToString(),
                CourseName = Course.Name,
                Provider = Provider.Name,
                Interviews = Interviews?.Select(c => c.ToCrmModel()).ToList(),
            };
        }
    }
}
