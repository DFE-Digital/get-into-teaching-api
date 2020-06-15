using Prometheus;

namespace GetIntoTeachingApi.Services
{
    public interface IMetricService
    {
        Histogram CrmSyncDuration { get; }
        Histogram LocationSyncDuration { get; }
        Gauge HangfireJobs { get; }
    }
}
