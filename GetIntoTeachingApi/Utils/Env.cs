namespace GetIntoTeachingApi.Utils
{
    public static class Env
    {
        public static bool IsDevelopment => System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        public static bool IsProduction => System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
        public static bool IsStaging => System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Staging";
    }
}
