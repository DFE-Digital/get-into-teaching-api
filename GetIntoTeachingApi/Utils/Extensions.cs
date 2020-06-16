using GetIntoTeachingApi.Services;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Prometheus;

namespace GetIntoTeachingApi.Utils
{
    public static class Extensions
    {
        public static IApplicationBuilder UsePrometheusHangfireExporter(this IApplicationBuilder app)
        {
            var jobStorage = (JobStorage)app.ApplicationServices.GetService(typeof(JobStorage));
            var metrics = (IMetricService)app.ApplicationServices.GetService(typeof(IMetricService));
            var exporter = new HangfirePrometheusExporter(jobStorage, metrics);

            Metrics.DefaultRegistry.AddBeforeCollectCallback(() => exporter.ExportHangfireStatistics());

            return app;
        }
    }
}
