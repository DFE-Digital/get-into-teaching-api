using System;
using GetIntoTeachingApi.Utils;

namespace GetIntoTeachingApi.Jobs
{
    public static class JobConfiguration
    {
        public static int Attempts => Env.IsDevelopment ? 5 : 24;
        public static int RetryIntervalInSeconds => Env.IsDevelopment ? 60 : 3600;
        public static TimeSpan ExpirationTimeout => TimeSpan.FromHours(24);
    }
}
