using GetIntoTeachingApi.Adapters;
using Hangfire.Server;

namespace GetIntoTeachingApi.Jobs
{
    public abstract class BaseJob
    {
        protected bool IsLastAttempt(PerformContext context, IPerformContextAdapter adapter)
        {
            var retryCount = adapter.GetRetryCount(context);
            return retryCount >= JobConfiguration.Attempts;
        }
    }
}
