using Prometheus;

namespace GetIntoTeachingApi.Services
{
    public interface IMetricService
    {
        Histogram CrmSyncDuration { get; }
        Histogram LocationSyncDuration { get; }
        Histogram LocationBatchDuration { get; }
        Histogram HangfireJobQueueDuration { get; }
        Gauge HangfireJobs { get; }
        Counter GoogleApiCalls { get; }
        Counter CacheLookups { get; }
    }
}
