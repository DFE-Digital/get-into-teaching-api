using System;
using System.Linq;
using Hangfire.Dashboard;

namespace GetIntoTeachingApi.Auth
{
    public class HangfireDashboardAuthroizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return new[] {"Development", "Staging"}.Contains(environment);
        }
    }
}
