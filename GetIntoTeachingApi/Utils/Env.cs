using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GetIntoTeachingApi.Utils
{
    public class Env : IEnv
    {
        public bool IsDevelopment => EnvironmentName == "Development";
        public bool IsProduction => EnvironmentName == "Production";
        public bool IsStaging => EnvironmentName == "Staging";
        public bool IsTest => EnvironmentName == "Test";
        public string GitCommitSha => Environment.GetEnvironmentVariable("GIT_COMMIT_SHA");
        public bool ExportHangireToPrometheus => InstanceIndex == "0";
        public string InstanceIndex => Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX");
        public string DatabaseInstanceName => Environment.GetEnvironmentVariable("DATABASE_INSTANCE_NAME");
        public string PgConnectionString => Environment.GetEnvironmentVariable("PG_CONN_STR");
        public string RedisConnectionString => Environment.GetEnvironmentVariable("REDIS_CONN_STR");
        public string AksEnvName => Environment.GetEnvironmentVariable("AKS_ENV_NAME");
        public bool EnableMetrics => Environment.GetEnvironmentVariable("ENABLE_METRICS") == "1";
        public string HangfireInstanceName => Environment.GetEnvironmentVariable("HANGFIRE_INSTANCE_NAME");
        public string HangfireUsername => Environment.GetEnvironmentVariable("HANGFIRE_USERNAME");
        public string HangfirePassword => Environment.GetEnvironmentVariable("HANGFIRE_PASSWORD");
        public string TotpSecretKey => Environment.GetEnvironmentVariable("TOTP_SECRET_KEY");
        public string VcapServices => Environment.GetEnvironmentVariable("VCAP_SERVICES");
        public string CrmServiceUrl => Environment.GetEnvironmentVariable("CRM_SERVICE_URL");
        public string CrmClientId => Environment.GetEnvironmentVariable("CRM_CLIENT_ID");
        public string CrmClientSecret => Environment.GetEnvironmentVariable("CRM_CLIENT_SECRET");
        public string NotifyApiKey => Environment.GetEnvironmentVariable("NOTIFY_API_KEY");
        public string GoogleApiKey => Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
        public string ApplyCandidateApiKey => Environment.GetEnvironmentVariable("APPLY_CANDIDATE_API_KEY");
        public string ApplyCandidateApiUrl => Environment.GetEnvironmentVariable("APPLY_CANDIDATE_API_URL");
        public string AppName => AppServices.ApplicationName;
        public string Organization => AppServices.OrganizationName;
        public string Space => AppServices.SpaceName;

        // The master instance boots first on deploy.
        public bool IsMasterInstance
        {
            get
            {
                var success = int.TryParse(InstanceIndex, out int value);

                return !success || value == 0;
            }
        }

        public string EnvironmentName
        {
            get
            {
                var name = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                return name ?? "Test";
            }
        }

        public string CloudFoundryEnvironmentName
        {
            get
            {
                return AppServices == null ? "local" : AppServices.ApplicationName.Split("-").Last();
            }
        }

        public bool IsFeatureOn(string feature)
        {
            var value = Environment.GetEnvironmentVariable($"{feature}_FEATURE");

            return value.ToBool();
        }

        public bool IsFeatureOff(string feature)
        {
            return !IsFeatureOn(feature);
        }

        public string GetVariable(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }

        private static ApplicationServices AppServices
        {
            get
            {
                var vcapApplication = Environment.GetEnvironmentVariable("VCAP_APPLICATION");
                return vcapApplication == null ? null : JsonSerializer.Deserialize<ApplicationServices>(vcapApplication);
            }
        }

        private sealed class ApplicationServices
        {
            [JsonPropertyName("application_name")]
            public string ApplicationName { get; set; }
            [JsonPropertyName("organization_name")]
            public string OrganizationName { get; set; }
            [JsonPropertyName("space_name")]
            public string SpaceName { get; set; }
        }
    }
}
