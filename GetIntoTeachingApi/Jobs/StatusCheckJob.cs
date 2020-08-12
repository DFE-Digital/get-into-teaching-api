using GetIntoTeachingApi.Utils;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Jobs
{
    public class StatusCheckJob : BaseJob
    {
        private readonly ILogger<StatusCheckJob> _logger;

        public StatusCheckJob(IEnv env, ILogger<StatusCheckJob> logger)
            : base(env)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation("Hangfire - Status Check");
        }
    }
}
