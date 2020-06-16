using System;
using Hangfire;
using Prometheus;

namespace GetIntoTeachingApi.Services
{
    public class HangfirePrometheusExporter : IHangfirePrometheusExporter
    {
        private readonly JobStorage _hangfireJobStorage;
        private readonly IMetricService _metrics;
        private const string RetrySetName = "retries";

        public HangfirePrometheusExporter(JobStorage hangfireJobStorage, IMetricService metrics)
        {
            _hangfireJobStorage = hangfireJobStorage;
            _metrics = metrics;
        }

        public void ExportHangfireStatistics()
        {
            try
            {
                var metric = _metrics.HangfireJobs;
                var jobStatistics = GetJobStatistics();

                metric.WithLabels("deleted").Set(jobStatistics.Deleted);
                metric.WithLabels("failed").Set(jobStatistics.Failed);
                metric.WithLabels("scheduled").Set(jobStatistics.Scheduled);
                metric.WithLabels("processing").Set(jobStatistics.Processing);
                metric.WithLabels("enqueued").Set(jobStatistics.Enqueued);
                metric.WithLabels("succeeded").Set(jobStatistics.Succeeded);
                metric.WithLabels("retry").Set(jobStatistics.Retry);
            }
            catch (Exception exception)
            {
                throw new ScrapeFailedException("HangfirePrometheusExporter - Scrape failed, inner exception:", exception);
            }
        }

        private HangfireJobStatistics GetJobStatistics()
        {
            var hangfireStats = _hangfireJobStorage.GetMonitoringApi().GetStatistics();
            long retryJobs = _hangfireJobStorage.GetConnection().GetAllItemsFromSet(RetrySetName).Count;

            return new HangfireJobStatistics
            {
                Deleted = hangfireStats.Deleted,
                Failed = hangfireStats.Failed,
                Enqueued = hangfireStats.Enqueued,
                Scheduled = hangfireStats.Scheduled,
                Processing = hangfireStats.Processing,
                Succeeded = hangfireStats.Succeeded,
                Retry = retryJobs
            };
        }

        internal class HangfireJobStatistics
        {
            public long Deleted { get; set; }
            public long Enqueued { get; set; }
            public long Scheduled { get; set; }
            public long Processing { get; set; }
            public long Succeeded { get; set; }
            public long Failed { get; set; }
            public long Retry { get; set; }
        }
    }
}
