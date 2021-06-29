using AspNetCoreRateLimit;
using GetIntoTeachingApi.AppStart.Hangfire;
using GetIntoTeachingApi.AppStart.Prometheus;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace GetIntoTeachingApi.AppStart
{
    public class StartupProduction : Startup
    {
        public StartupProduction(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app)
        {
            PrometheusMetricLabels.SetLabels(Env);

            app.UseClientRateLimiting();

            var hangfireOptions = new BackgroundJobServerOptions
            {
                WorkerCount = 20,
            };
            app.UseHangfireServer(hangfireOptions);

            HangfireJobs.AddCrmSyncJob();
            HangfireJobs.AddLocationSyncJob();
            HangfireJobs.AddMagicLinkTokenGenerationJob();

            base.Configure(app);
        }
    }
}
