using System;
using Hangfire.Server;

namespace GetIntoTeachingApi.Adapters
{
    public interface IPerformContextAdapter
    {
        int GetRetryCount(PerformContext context);
        DateTime GetJobCreatedAt(PerformContext context);
    }
}
