using System;

namespace GetIntoTeachingApi.Services.Crm
{
    public class ODataCredentials : IODataCredentials
    {
        public string ServiceUrl() => Environment.GetEnvironmentVariable("CRM_SERVICE_URL");
        public string TenantId() => Environment.GetEnvironmentVariable("CRM_TENANT_ID");
        public string ClientId() => Environment.GetEnvironmentVariable("CRM_CLIENT_ID");
        public string Secret() => Environment.GetEnvironmentVariable("CRM_CLIENT_SECRET");
        public string AuthUrl() => $"https://login.microsoftonline.com/{TenantId()}";
    }
}
