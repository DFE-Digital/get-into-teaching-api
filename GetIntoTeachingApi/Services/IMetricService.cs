using Prometheus;

namespace GetIntoTeachingApi.Services
{
    public interface IMetricService
    {
        Histogram CrmSyncDuration { get; }
        Histogram LocationSyncDuration { get; }
        Histogram MagicLinkTokenGenerationDuration { get; }
        Histogram HangfireJobQueueDuration { get; }
        Histogram TeachingEventSearchResults { get; }
        Gauge HangfireJobs { get; }
        Counter GoogleApiCalls { get; }
        Counter CacheLookups { get; }
        Counter VerifiedTotps { get; }
        Counter GeneratedTotps { get; }
    }
}
