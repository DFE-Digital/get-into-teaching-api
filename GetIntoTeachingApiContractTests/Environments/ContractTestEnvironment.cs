using System;

namespace GetIntoTeachingApiContractTests.Environments
{
    public static class ContractTestEnvironment
    {
        public static void Setup(string sharedSecret, string crmServiceUrl, string crmClientId, string crmClientSecret)
        {
            Environment.SetEnvironmentVariable("SHARED_SECRET", sharedSecret);
            Environment.SetEnvironmentVariable("CRM_SERVICE_URL", crmServiceUrl);
            Environment.SetEnvironmentVariable("CRM_CLIENT_ID", crmClientId);
            Environment.SetEnvironmentVariable("CRM_CLIENT_SECRET", crmClientSecret);
        }
    }
}