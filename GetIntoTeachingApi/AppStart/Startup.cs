using System.IO;
using System.Text.Json.Serialization;
using AspNetCoreRateLimit;
using dotenv.net;
using FluentValidation.AspNetCore;
using GetIntoTeachingApi.JsonConverters;
using GetIntoTeachingApi.ModelBinders;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace GetIntoTeachingApi.AppStart
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IEnv _env;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _env = new Env();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (!_env.IsTest)
            {
                var root = Directory.GetCurrentDirectory();
                var envFile = Path.Combine(root, $"env.{_env.CloudFoundryEnvironmentName.ToLowerInvariant()}");
                DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false, envFilePaths: new[] { envFile }));
            }

            services.RegisterServices(_configuration, _env);

            services.AddDatabase(_env);

            services.AddApiClientAuthentication();

            services.ConfigureRedis(_env);

            services.AddMvc(o => o.Conventions.Add(new CommaSeparatedQueryStringConvention()));

            services
                .AddControllers(o => o.ModelBinderProviders.Insert(0, new TrimStringModelBinderProvider()))
                .AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    o.JsonSerializerOptions.Converters.Add(new TrimStringJsonConverter());
                    o.JsonSerializerOptions.Converters.Add(new EmptyStringToNullJsonConverter());
                });

            services.Configure<KestrelServerOptions>(options =>
            {
                // Workaround for https://github.com/dotnet/aspnetcore/issues/8302
                // caused by Prometheus.HttpMetrics.HttpRequestDurationMiddleware
                options.AllowSynchronousIO = true;
            });

            services.AddSwagger();

            services.AddHangfire(_env, useMemoryStorage: _env.IsTest);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            // We can't do this in test as the DefaultRegistry is shared between runs
            // and we get an error trying to set static labels once metrics have been registed.
            // There doesn't appear to be a way to clear the DefaultRegistry between tests.
            if (!_env.IsTest)
            {
                PrometheusMetricLabels.SetLabels(_env);
            }

            if (!_env.IsStaging)
            {
                app.UseClientRateLimiting();
            }

            if (_env.IsDevelopment)
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRequestResponseLogging();

            app.ConfigureHangfire(addBasicAuthFilter: _env.IsStaging, _env);

            app.UseSwagger();

            if (!_env.IsProduction)
            {
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Get into Teaching API V1"));
            }

            app.UseRouting();

            if (_env.ExportHangireToPrometheus)
            {
                app.UsePrometheusHangfireExporter();
            }

            app.UseHttpMetrics();

            app.UseAuthorization();

            ResponseHeaders.SetupSecureHeaders(app);

            DatabaseUtility.Migrate(serviceScope, _env);

            if (!_env.IsTest)
            {
                HangfireJobs.AddCrmSyncJob();
                HangfireJobs.AddLocationSyncJob();
                HangfireJobs.AddMagicLinkTokenGenerationJob();

                if (_env.IsFeatureOn("APPLY_API"))
                {
                    HangfireJobs.AddFindApplySyncJob();
                }
            }

            if (!_env.IsTest)
            {
                DatabaseUtility.Seed(serviceScope);
            }

            // Configure rate limiting.
            var clientPolicyStore = serviceScope.ServiceProvider.GetRequiredService<IClientPolicyStore>();
            clientPolicyStore.SeedAsync().GetAwaiter().GetResult();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapControllers();
            });
        }
    }
}
