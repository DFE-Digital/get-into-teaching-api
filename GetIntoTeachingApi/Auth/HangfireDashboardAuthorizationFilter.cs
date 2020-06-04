using System.Linq;
using GetIntoTeachingApi.Utils;
using Hangfire.Dashboard;

namespace GetIntoTeachingApi.Auth
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IEnv _env;

        public HangfireDashboardAuthorizationFilter(IEnv env)
        {
            _env = env;
        }

        public bool Authorize(DashboardContext context)
        {
            return new[] {"Development", "Staging"}.Contains(_env.EnvironmentName);
        }
    }
}
