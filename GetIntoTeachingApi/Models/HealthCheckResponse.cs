using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Attributes;

namespace GetIntoTeachingApi.Models
{
    public class HealthCheckResponse
    {
        public const string StatusOk = "ok";
        public string GitCommitSha { get; set; }
        public string Environment { get; set; }
        public string Database { get; set; }
        public string Hangfire { get; set; }
        public string Crm { get; set; }
        public string Redis { get; set; }
        public string Notify { get; set; }
        [JsonIgnore]
        public IEnumerable<string> Services => CriticalServices.Concat(NonCriticalServices);
        [JsonIgnore]
        public IEnumerable<string> CriticalServices => new[] { Database, Hangfire };
        [JsonIgnore]
        public IEnumerable<string> NonCriticalServices => new[] { Crm, Notify, Redis };

        public string Status
        {
            get
            {
                if (Services.All(s => s == StatusOk))
                {
                    return "healthy";
                }

                if (NonCriticalServices.Any(s => s != StatusOk) && CriticalServices.All(s => s == StatusOk))
                {
                    return "degraded";
                }

                return "unhealthy";
            }
        }
    }
}
