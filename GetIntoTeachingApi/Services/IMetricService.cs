using Prometheus;

namespace GetIntoTeachingApi.Services
{
    public interface IMetricService
    {
        Histogram CrmSyncDuration { get; }
        Histogram LocationSyncDuration { get; }
        Histogram LocationBatchDuration { get; }
        Gauge HangfireJobs { get; }
    }
}
