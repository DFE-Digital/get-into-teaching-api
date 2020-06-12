using Prometheus;

namespace GetIntoTeachingApi.Services
{
    public class MetricService : IMetricService
    {
        private static readonly Histogram _crmSyncDuration = Metrics
            .CreateHistogram("api_crm_sync_duration_seconds", "Histogram of CRM sync durations.");

        public Histogram CrmSyncDuration => _crmSyncDuration;
    }
}
