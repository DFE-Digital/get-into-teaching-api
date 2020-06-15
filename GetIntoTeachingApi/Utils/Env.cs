using System;

namespace GetIntoTeachingApi.Utils
{
    public class Env : IEnv
    {
        public bool IsDevelopment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        public bool IsProduction => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
        public bool IsStaging => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Staging";
        public bool ExportHangireToPrometheus => Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX") == "0";
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
