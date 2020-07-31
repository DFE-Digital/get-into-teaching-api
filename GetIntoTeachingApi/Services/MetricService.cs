using Prometheus;

namespace GetIntoTeachingApi.Services
{
    public class MetricService : IMetricService
    {
        private static readonly Histogram _crmSyncDuration = Metrics
            .CreateHistogram("api_crm_sync_duration_seconds", "Histogram of CRM sync durations.");
        private static readonly Histogram _locationSyncDuration = Metrics
            .CreateHistogram("api_location_sync_duration_seconds", "Histogram of location sync durations.");
        private static readonly Histogram _locationBatchDuration = Metrics
            .CreateHistogram("api_location_batch_duration_seconds", "Histogram of location batch processing durations.");
        private static readonly Gauge _hangfireJobs = Metrics
            .CreateGauge("api_hangfire_jobs", "Gauge number of Hangifre jobs.", "state");
        private static readonly Counter _googleApiCalls = Metrics
            .CreateCounter("api_google_pai_calls", "Number of Google API calls.");

        public Histogram CrmSyncDuration => _crmSyncDuration;
        public Histogram LocationSyncDuration => _locationSyncDuration;
        public Histogram LocationBatchDuration => _locationBatchDuration;
        public Gauge HangfireJobs => _hangfireJobs;
        public Counter GoogleApiCalls => _googleApiCalls;
    }
}
