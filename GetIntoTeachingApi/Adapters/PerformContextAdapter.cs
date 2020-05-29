using Hangfire.Server;

namespace GetIntoTeachingApi.Adapters
{
    public class PerformContextAdapter : IPerformContextAdapter
    {
        public int GetRetryCount(PerformContext context)
        {
            return context.GetJobParameter<int>("RetryCount");
        }
    }
}
