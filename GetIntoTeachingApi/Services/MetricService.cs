using Prometheus;

namespace GetIntoTeachingApi.Services
{
    public class MetricService : IMetricService
    {
        private static readonly Histogram _crmSyncDuration = Metrics
            .CreateHistogram("api_crm_sync_duration_seconds", "Histogram of CRM sync durations.");
        private static readonly Gauge _hangfireJobs = Metrics
            .CreateGauge("api_hangfire_jobs", "Gauge number of Hangifre jobs.", "state");

        public Histogram CrmSyncDuration => _crmSyncDuration;
        public Gauge HangfireJobs => _hangfireJobs;
    }
}
