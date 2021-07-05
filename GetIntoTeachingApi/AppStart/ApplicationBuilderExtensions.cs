using System.Collections.Generic;
using GetIntoTeachingApi.Auth;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Hangfire.Dashboard;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Prometheus;

namespace GetIntoTeachingApi.AppStart
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureHangfire(this IApplicationBuilder app, int? workerCount, bool addBasicAuthFilter, IEnv env)
        {
            var hangfireOptions = new BackgroundJobServerOptions();
            if (workerCount.HasValue)
            {
                hangfireOptions.WorkerCount = workerCount.Value;
            }

            app.UseHangfireServer(hangfireOptions);

            var filters = new List<IDashboardAuthorizationFilter> { new HangfireDashboardEnvironmentAuthorizationFilter(env) };

            if (addBasicAuthFilter)
            {
                var basicAuthFilter = new HangfireCustomBasicAuthenticationFilter()
                {
                    User = env.HangfireUsername,
                    Pass = env.HangfirePassword,
                };

                filters.Add(basicAuthFilter);
            }

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = filters,
            });
        }

        public static void UsePrometheusHangfireExporter(this IApplicationBuilder app)
        {
            var jobStorage = (JobStorage)app.ApplicationServices.GetService(typeof(JobStorage));
            var metrics = (IMetricService)app.ApplicationServices.GetService(typeof(IMetricService));
            var exporter = new HangfirePrometheusExporter(jobStorage, metrics);

            Metrics.DefaultRegistry.AddBeforeCollectCallback(() => exporter.ExportHangfireStatistics());
        }
    }
}
