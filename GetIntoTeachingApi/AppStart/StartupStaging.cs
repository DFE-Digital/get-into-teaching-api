using GetIntoTeachingApi.AppStart.Hangfire;
using GetIntoTeachingApi.AppStart.Prometheus;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GetIntoTeachingApi.AppStart
{
    public class StartupStaging : Startup
    {
        public StartupStaging(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app)
        {
            PrometheusMetricLabels.SetLabels(Env);

            var hangfireOptions = new BackgroundJobServerOptions
            {
                WorkerCount = 20,
            };
            app.UseHangfireServer(hangfireOptions);

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Get into Teaching API V1"));

            HangfireJobs.AddCrmSyncJob();
            HangfireJobs.AddLocationSyncJob();
            HangfireJobs.AddMagicLinkTokenGenerationJob();
            HangfireJobs.AddFindApplySyncJob();

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
