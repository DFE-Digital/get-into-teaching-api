using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using GetIntoTeachingApi.Services;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Moq;
using Prometheus;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class HangfirePrometheusExporterTests
    {
        private readonly Mock<IStorageConnection> _mockStorageConnection;
        private readonly Mock<IMonitoringApi> _mockMonitoringApi;
        private readonly HangfirePrometheusExporter _exporter;

        public HangfirePrometheusExporterTests()
        {
            _mockStorageConnection = new Mock<IStorageConnection>();
            _mockMonitoringApi = new Mock<IMonitoringApi>();

            var mockStorage = new Mock<JobStorage>();
            mockStorage.Setup(x => x.GetConnection()).Returns(_mockStorageConnection.Object);
            mockStorage.Setup(x => x.GetMonitoringApi()).Returns(_mockMonitoringApi.Object);

            _exporter = new HangfirePrometheusExporter(mockStorage.Object, new MetricService());
        }

        [Fact]
        public void ExportHangfireStatistics_PublishesToPrometheus()
        {
            var expectedStatistics = new StatisticsDto()
            {
                Failed = 1,
                Deleted = 2,
                Enqueued = 3,
                Processing = 4,
                Recurring = 5,
                Scheduled = 6,
                Succeeded = 7
            };
            var expectedRetrySet = new HashSet<string> { "job1", "job2", "job3" };

            _mockMonitoringApi.Setup(x => x.GetStatistics()).Returns(expectedStatistics);
            _mockStorageConnection.Setup(x => x.GetAllItemsFromSet("retries")).Returns(expectedRetrySet);

            _exporter.ExportHangfireStatistics();

            var prometheusContent = GetPrometheusContent();

            ExpectedMetricStrings(expectedStatistics, expectedRetrySet)
                .ForEach(ms => prometheusContent.Should().Contain(ms));
        }

        [Fact]
        public void ExportHangfireStatistics_OnException_ThrowsScrapeFailedException()
        {
            _mockMonitoringApi.Setup(x => x.GetStatistics()).Throws(new Exception());

            Action action = () => _exporter.ExportHangfireStatistics();;

            action.Should().Throw<ScrapeFailedException>()
                .And.Message.Should().Be("HangfirePrometheusExporter - Scrape failed, inner exception:");
        }

        private static List<string> ExpectedMetricStrings(StatisticsDto statistics, ICollection<string> retrySet)
        {
            return new List<string>
            {
                "# HELP api_hangfire_jobs Gauge number of Hangifre jobs.\n# TYPE api_hangfire_jobs gauge",
                MetricString("failed", statistics.Failed),
                MetricString("deleted", statistics.Deleted),
                MetricString("enqueued", statistics.Enqueued),
                MetricString("scheduled", statistics.Scheduled),
                MetricString("processing", statistics.Processing),
                MetricString("succeeded", statistics.Succeeded),
                MetricString("retry", retrySet.Count)
            };
        }

        private static string MetricString(string labelValue, double metricValue)
        {
            return $"api_hangfire_jobs{{state=\"{labelValue}\"}} {metricValue}";
        }

        private static string GetPrometheusContent()
        {
            using var memoryStream = new MemoryStream();
            Metrics.DefaultRegistry.CollectAndExportAsTextAsync(memoryStream).Wait();
            memoryStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(memoryStream);
            return reader.ReadToEnd();
        }
    }
}
