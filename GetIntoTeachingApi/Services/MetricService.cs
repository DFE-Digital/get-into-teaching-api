using Prometheus;

namespace GetIntoTeachingApi.Services
{
    public class MetricService : IMetricService
    {
        private static readonly Histogram _crmSyncDuration = Metrics
            .CreateHistogram("api_crm_sync_duration_seconds", "Histogram of CRM sync durations.");
        private static readonly Histogram _locationSyncDuration = Metrics
            .CreateHistogram("api_location_sync_duration_seconds", "Histogram of location sync durations.");
        private static readonly Histogram _hangfireJobQueueDuration = Metrics
            .CreateHistogram("api_hangfire_job_queue_duration_seconds", "Histogram of the time jobs spend in the queue.", new HistogramConfiguration
            {
                LabelNames = new[] { "job" },
            });
        private static readonly Gauge _hangfireJobs = Metrics
            .CreateGauge("api_hangfire_jobs", "Gauge number of Hangifre jobs.", "state");
        private static readonly Counter _googleApiCalls = Metrics
            .CreateCounter("api_google_api_calls", "Number of Google API calls.", new CounterConfiguration
            {
                LabelNames = new[] { "postcode", "result" },
            });
        private static readonly Counter _cacheLookups = Metrics
            .CreateCounter("api_cache_lookups", "Number of cache lookups.", new CounterConfiguration
            {
                LabelNames = new[] { "outcome" },
            });
        private static readonly Counter _generatedTotps = Metrics
            .CreateCounter("api_generated_totps", "Number of generated timed one time passwords.", new CounterConfiguration
            {
                LabelNames = new[] { "candidate_id", "totp" },
            });
        private static readonly Counter _verifiedTotps = Metrics
            .CreateCounter("api_verified_totps", "Number of verified timed one time passwords.", new CounterConfiguration
            {
                LabelNames = new[] { "candidate_id", "totp", "valid" },
            });

        public Histogram CrmSyncDuration => _crmSyncDuration;
        public Histogram LocationSyncDuration => _locationSyncDuration;
        public Histogram HangfireJobQueueDuration => _hangfireJobQueueDuration;
        public Gauge HangfireJobs => _hangfireJobs;
        public Counter GoogleApiCalls => _googleApiCalls;
        public Counter CacheLookups => _cacheLookups;
        public Counter GeneratedTotps => _generatedTotps;
        public Counter VerifiedTotps => _verifiedTotps;
    }
}
