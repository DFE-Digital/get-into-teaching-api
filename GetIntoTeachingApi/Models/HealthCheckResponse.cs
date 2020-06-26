using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

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
        public string Notify { get; set; }
        [JsonIgnore]
        public IEnumerable<string> Services => DependentServices.Concat(DownstreamServices);
        [JsonIgnore]
        public IEnumerable<string> DependentServices => new[] { Database, Hangfire };
        [JsonIgnore]
        public IEnumerable<string> DownstreamServices => new[] { Crm, Notify };

        public string Status
        {
            get
            {
                if (Services.All(s => s == StatusOk))
                {
                    return "healthy";
                }

                if (DownstreamServices.Any(s => s != StatusOk) && DependentServices.All(s => s == StatusOk))
                {
                    return "degraded";
                }

                return "unhealthy";
            }
        }
    }
}
