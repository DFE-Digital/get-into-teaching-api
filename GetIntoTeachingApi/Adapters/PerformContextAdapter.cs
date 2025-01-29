using System;
using Hangfire.Server;

namespace GetIntoTeachingApi.Adapters
{
    public class PerformContextAdapter : IPerformContextAdapter
    {
        public int GetRetryCount(PerformContext context)
        {
            return context.GetJobParameter<int>("RetryCount");
        }

        public DateTime GetJobCreatedAt(PerformContext context)
        {
            return context.BackgroundJob.CreatedAt;
        }
    }
}
