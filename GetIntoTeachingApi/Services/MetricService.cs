using Prometheus;

namespace GetIntoTeachingApi.Services
{
    public class MetricService : IMetricService
    {
        private static readonly Histogram _crmSyncDuration = Metrics
            .CreateHistogram("api_crm_sync_duration_seconds", "Histogram of CRM sync durations.");
        private static readonly Histogram _locationSyncDuration = Metrics
            .CreateHistogram("api_location_sync_duration_seconds", "Histogram of location sync durations.");
        private static readonly Gauge _hangfireJobs = Metrics
            .CreateGauge("api_hangfire_jobs", "Gauge number of Hangifre jobs.", "state");

        public Histogram CrmSyncDuration => _crmSyncDuration;
        public Histogram LocationSyncDuration => _locationSyncDuration;
        public Gauge HangfireJobs => _hangfireJobs;
    }
}
