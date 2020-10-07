using System;
using GetIntoTeachingApi.Utils;
using Microsoft.PowerPlatform.Cds.Client;

namespace GetIntoTeachingApi.Adapters
{
    public class CdsServiceClientWrapper
    {
        public readonly CdsServiceClient CdsServiceClient;

        public CdsServiceClientWrapper(IEnv env)
        {
            CdsServiceClient = new CdsServiceClient(ConnectionString(env));
            CdsServiceClient.MaxConnectionTimeout = TimeSpan.FromSeconds(30);
        }

        private static string ConnectionString(IEnv env)
        {
            var instanceUrl = env.CrmServiceUrl;
            var clientId = env.CrmClientId;
            var clientSecret = env.CrmClientSecret;
            return $"AuthType=ClientSecret; url={instanceUrl}; ClientId={clientId}; ClientSecret={clientSecret}";
        }
    }
}
