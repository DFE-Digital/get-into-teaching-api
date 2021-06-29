using AspNetCoreRateLimit;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;


namespace GetIntoTeachingApi.AppStart
{
    public class StartupTest : Startup
    {
        public StartupTest(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseClientRateLimiting();

            var hangfireOptions = new BackgroundJobServerOptions
            {
                WorkerCount = 20,
            };
            app.UseHangfireServer(hangfireOptions);

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Get into Teaching API V1"));

            base.Configure(app);
        }
    }
}
