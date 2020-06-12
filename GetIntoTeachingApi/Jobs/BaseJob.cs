using GetIntoTeachingApi.Adapters;
using Hangfire.Server;

namespace GetIntoTeachingApi.Jobs
{
    public abstract class BaseJob
    {
        protected bool IsLastAttempt(PerformContext context, IPerformContextAdapter adapter)
        {
            var currentAttempt = CurrentAttempt(context, adapter);
            return currentAttempt >= JobConfiguration.Attempts;
        }

        protected int CurrentAttempt(PerformContext context, IPerformContextAdapter adapter)
        {
            return adapter.GetRetryCount(context) + 1;
        }

        protected string AttemptInfo(PerformContext context, IPerformContextAdapter adapter)
        {
            return $"{CurrentAttempt(context, adapter)}/{JobConfiguration.Attempts}";
        }
    }
}
