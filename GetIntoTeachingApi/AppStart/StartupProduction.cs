using AspNetCoreRateLimit;
using GetIntoTeachingApi.AppStart.Hangfire;
using GetIntoTeachingApi.AppStart.Prometheus;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            var scope = CreateScope(app);

            ConfigureDatabase(scope);

            ConfigureRateLimiting(scope);

            base.Configure(app);
        }

        private void ConfigureDatabase(IServiceScope scope)
        {
            var databaseUtility = new DatabaseUtility(scope);

            databaseUtility.Migrate(Env);

            databaseUtility.Seed();
        }
    }
}
