using FluentAssertions;
using GetIntoTeachingApi.Services;
using Prometheus;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class MetricServiceTests
    {
        private readonly IMetricService _metrics;

        public MetricServiceTests()
        {
            _metrics = new MetricService();
        }

        [Fact]
        public void CrmSyncDuration_ReturnsMetric()
        {
            _metrics.CrmSyncDuration.Name.Should().Be("api_crm_sync_duration_seconds");
        }

        [Fact]
        public void LocationSyncDuration_ReturnsMetric()
        {
            _metrics.LocationSyncDuration.Name.Should().Be("api_location_sync_duration_seconds");
        }

        [Fact]
        public void LocationBatchDuration_ReturnsMetric()
        {
            _metrics.LocationBatchDuration.Name.Should().Be("api_location_batch_duration_seconds");
        }

        [Fact]
        public void HangfireJobQueueDuration_ReturnsMetric()
        {
            _metrics.HangfireJobQueueDuration.Name.Should().Be("api_hangfire_job_queue_duration_seconds");
            _metrics.HangfireJobQueueDuration.LabelNames.Should().BeEquivalentTo(new[] { "job" });
        }

        [Fact]
        public void HangfireJobs_ReturnsMetric()
        {
            _metrics.HangfireJobs.Name.Should().Be("api_hangfire_jobs");
        }

        [Fact]
        public void GoogleApiCalls_ReturnsMetric()
        {
            _metrics.GoogleApiCalls.Name.Should().Be("api_google_api_calls");
            _metrics.GoogleApiCalls.LabelNames.Should().BeEquivalentTo(new[] { "postcode", "result" });
        }

        [Fact]
        public void CacheLookups_ReturnsMetric()
        {
            _metrics.CacheLookups.Name.Should().Be("api_cache_lookups");
            _metrics.CacheLookups.LabelNames.Should().BeEquivalentTo(new[] { "outcome" });
        }
    }
}
