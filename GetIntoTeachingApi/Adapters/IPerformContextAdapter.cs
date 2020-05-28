using Hangfire.Server;

namespace GetIntoTeachingApi.Adapters
{
    public interface IPerformContextAdapter
    {
        int GetRetryCount(PerformContext context);
    }
}
