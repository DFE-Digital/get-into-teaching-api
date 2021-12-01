using FluentAssertions;
using GetIntoTeachingApi.Services;
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
        public void FindApplySyncDuration_ReturnsMetric()
        {
            _metrics.FindApplySyncDuration.Name.Should().Be("api_find_apply_sync_duration_seconds");
        }

        [Fact]
        public void LocationSyncDuration_ReturnsMetric()
        {
            _metrics.LocationSyncDuration.Name.Should().Be("api_location_sync_duration_seconds");
        }

        [Fact]
        public void MagicLinkTokenGenerationDuration_ReturnsMetric()
        {
            _metrics.MagicLinkTokenGenerationDuration.Name.Should().Be("api_magic_link_token_generation_duration_seconds");
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
            _metrics.GoogleApiCalls.LabelNames.Should().BeEquivalentTo(new[] { "result" });
        }

        [Fact]
        public void CacheLookups_ReturnsMetric()
        {
            _metrics.CacheLookups.Name.Should().Be("api_cache_lookups");
            _metrics.CacheLookups.LabelNames.Should().BeEquivalentTo(new[] { "outcome" });
        }

        [Fact]
        public void GeneratedTotps_ReturnsMetric()
        {
            _metrics.GeneratedTotps.Name.Should().Be("api_generated_totps");
            _metrics.GeneratedTotps.LabelNames.Should().BeEquivalentTo(new[] { "reference" });
        }

        [Fact]
        public void VerifiedTotps_ReturnsMetric()
        {
            _metrics.VerifiedTotps.Name.Should().Be("api_verified_totps");
            _metrics.VerifiedTotps.LabelNames.Should().BeEquivalentTo(new[] { "valid", "reference" });
        }

        [Fact]
        public void TeachingEventSearchResults_ReturnsMetric()
        {
            _metrics.TeachingEventSearchResults.Name.Should().Be("api_teaching_event_search_results_count");
            _metrics.TeachingEventSearchResults.LabelNames.Should().BeEquivalentTo(new[] { "type_ids", "radius" });
        }


        [Fact]
        public void InPersonTeachingEventResults_ReturnsMetric()
        {
            _metrics.InPersonTeachingEventResults.Name.Should().Be("api_in_person_teaching_event_results_count");
            _metrics.InPersonTeachingEventResults.LabelNames.Should().BeEquivalentTo(new[] { "type_ids", "radius" });
        }
    }
}
