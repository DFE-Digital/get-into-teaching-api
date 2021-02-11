using System;
using GetIntoTeachingApi.Utils;

namespace GetIntoTeachingApi.Jobs
{
    public static class JobConfiguration
    {
        public static int Attempts(IEnv env) => env.IsDevelopment ? 5 : 24;
        public static int RetryIntervalInSeconds(IEnv env) => env.IsDevelopment ? 60 : 3600;
        public static TimeSpan ExpirationTimeout => TimeSpan.FromHours(24);
        public static string CrmSyncJobId => "crm-sync";
        public static string LocationSyncJobId => "location-sync";
        public static string MagicLinkTokenGenerationJobId => "magic-link-token-generation";
    }
}
