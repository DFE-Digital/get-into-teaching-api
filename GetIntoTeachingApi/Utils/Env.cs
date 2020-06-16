using System;

namespace GetIntoTeachingApi.Utils
{
    public class Env : IEnv
    {
        public bool IsDevelopment => EnvironmentName == "Development";
        public bool IsProduction => EnvironmentName == "Production";
        public bool IsStaging => EnvironmentName == "Staging";
        public bool ExportHangireToPrometheus => Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX") == "0";
        public string DatabaseInstanceName => Environment.GetEnvironmentVariable("DATABASE_INSTANCE_NAME");
        public string HangfireInstanceName => Environment.GetEnvironmentVariable("HANGFIRE_INSTANCE_NAME");
        public string SentryUrl => Environment.GetEnvironmentVariable("SENTRY_URL");
        public string EnvironmentName => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        public string TotpSecretKey => Environment.GetEnvironmentVariable("TOTP_SECRET_KEY");
        public string VcapServices => Environment.GetEnvironmentVariable("VCAP_SERVICES");
        public string CrmServiceUrl => Environment.GetEnvironmentVariable("CRM_SERVICE_URL");
        public string CrmClientId => Environment.GetEnvironmentVariable("CRM_CLIENT_ID");
        public string CrmClientSecret => Environment.GetEnvironmentVariable("CRM_CLIENT_SECRET");
        public string NotifyApiKey => Environment.GetEnvironmentVariable("NOTIFY_API_KEY");
        public string SharedSecret => Environment.GetEnvironmentVariable("SHARED_SECRET");
    }
}
