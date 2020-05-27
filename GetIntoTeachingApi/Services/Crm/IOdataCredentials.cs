using System;

namespace GetIntoTeachingApi.Services.Crm
{
    public interface IODataCredentials
    {
        public string ServiceUrl();
        public Uri ODataServiceUri();
        public string TenantId();
        public string ClientId();
        public string Secret();
        public string AuthUrl();
    }
}
