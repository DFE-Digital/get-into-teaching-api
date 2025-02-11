using System.Linq;
using GetIntoTeachingApi.Utils;
using Hangfire.Dashboard;

namespace GetIntoTeachingApi.Auth
{
    public class HangfireDashboardEnvironmentAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IEnv _env;
        private static readonly string[] sourceArray = [ "Development", "Staging" ];

        public HangfireDashboardEnvironmentAuthorizationFilter(IEnv env)
        {
            _env = env;
        }

        public bool Authorize(DashboardContext context)
        {
            return sourceArray.Contains(_env.EnvironmentName);
        }
    }
}
