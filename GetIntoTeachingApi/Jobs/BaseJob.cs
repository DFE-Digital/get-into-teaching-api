using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Utils;
using Hangfire.Server;

namespace GetIntoTeachingApi.Jobs
{
    public abstract class BaseJob
    {
        protected readonly IEnv Env;

        protected BaseJob(IEnv env)
        {
            Env = env;
        }

        protected bool IsLastAttempt(PerformContext context, IPerformContextAdapter adapter)
        {
            var currentAttempt = CurrentAttempt(context, adapter);
            return currentAttempt >= JobConfiguration.Attempts(Env);
        }

        protected int CurrentAttempt(PerformContext context, IPerformContextAdapter adapter)
        {
            return adapter.GetRetryCount(context) + 1;
        }

        protected string AttemptInfo(PerformContext context, IPerformContextAdapter adapter)
        {
            return $"{CurrentAttempt(context, adapter)}/{JobConfiguration.Attempts(Env)}";
        }
    }
}
