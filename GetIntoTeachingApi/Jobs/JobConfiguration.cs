using System;

namespace GetIntoTeachingApi.Jobs
{
    public static class JobConfiguration
    {
        private static bool IsDevelopment =>
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        public static int Attempts => IsDevelopment ? 5 : 24;
        public static int RetryIntervalInSeconds => IsDevelopment ? 60 : 3600;
        public static TimeSpan ExpirationTimeout => TimeSpan.FromHours(24);
    }
}
