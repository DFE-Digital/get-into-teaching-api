using System;
using GetIntoTeachingApi.Utils;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace GetIntoTeachingApi.Adapters
{
    public class CdsServiceClientWrapper : IDisposable
    {
        internal readonly ServiceClient CdsServiceClient;
        private bool disposed;

        public CdsServiceClientWrapper(IEnv env)
        {
            // We don't want to try and connect to Dynamics when integration testing.
            if (!env.IsTest)
            {
                ServiceClient.MaxConnectionTimeout = TimeSpan.FromSeconds(30);
                CdsServiceClient = new ServiceClient(ConnectionString(env));
                CdsServiceClient.MaxRetryCount = 3;
            }
        }

        private static string ConnectionString(IEnv env)
        {
            var instanceUrl = env.CrmServiceUrl;
            var clientId = env.CrmClientId;
            var clientSecret = env.CrmClientSecret;
            return $"AuthType=ClientSecret; url={instanceUrl}; ClientId={clientId}; ClientSecret={clientSecret}";
        }

        public void Dispose()
        {
            Dispose(true);   
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    CdsServiceClient.Dispose();
                }

                disposed = true;
            }
        }
    }
}
