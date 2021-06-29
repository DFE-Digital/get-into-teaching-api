using AspNetCoreRateLimit;
using dotenv.net;
using GetIntoTeachingApi.AppStart.Hangfire;
using GetIntoTeachingApi.AppStart.Prometheus;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GetIntoTeachingApi.AppStart
{
    public class StartupDevelopment : Startup
    {
        public StartupDevelopment(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            DotEnv.Config(true, ".env.development");
        }

        public override void Configure(IApplicationBuilder app)
        {
            PrometheusMetricLabels.SetLabels(Env);

            app.UseClientRateLimiting();

            app.UseDeveloperExceptionPage();

            app.UseHangfireServer();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Get into Teaching API V1"));

            HangfireJobs.AddCrmSyncJob();
            HangfireJobs.AddLocationSyncJob();
            HangfireJobs.AddMagicLinkTokenGenerationJob();
            HangfireJobs.AddFindApplySyncJob();

            base.Configure(app);
        }
    }
}
