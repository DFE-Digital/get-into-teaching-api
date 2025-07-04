﻿using AspNetCoreRateLimit;
using dotenv.net;
using FluentValidation;
using FluentValidation.AspNetCore;
using GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.Middleware;
using GetIntoTeachingApi.JsonConverters;
using GetIntoTeachingApi.ModelBinders;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using System.IO;
using System.Text.Json.Serialization;

namespace GetIntoTeachingApi.AppStart
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly Env _env;

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
                var envName = _env.CloudFoundryEnvironmentName.ToLowerInvariant();
                if (!string.IsNullOrEmpty(_env.AksEnvName)) {
                  envName = _env.AksEnvName;
                }
                var envFile = Path.Combine(root, $"env.{envName}");
                if (File.Exists(envFile)) {
                  DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false, envFilePaths: new[] { envFile }));
                }
            }

            services.RegisterServices(_configuration, _env);
            
            services.AddDatabase(_env);

            services.AddApiClientAuthentication();

            services.ConfigureRedis(_env);

            services.AddMvc(o => o.Conventions.Add(new CommaSeparatedQueryStringConvention()));

            services
                .AddValidatorsFromAssemblyContaining<Startup>()
                .AddFluentValidationAutoValidation()
                .AddControllers(o => o.ModelBinderProviders.Insert(0, new TrimStringModelBinderProvider()))
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

            // This registration provides the composition root for the degree-status inference services.
            // This is a temporary change to facilitate the way we capture DegreeStatus and graduation year
            // so we can better segment different year groups. This inference logic allows us to temporarily
            // maintain the DegreeStatusId field until we fully transition to graduation year only. 
            services.RegisterDegreeStatusInferenceServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            PrometheusMetrics.Configure();

            // We can't do this in test as the DefaultRegistry is shared between runs
            // and we get an error trying to set static labels once metrics have been registed.
            // There doesn't appear to be a way to clear the DefaultRegistry between tests.
            if (!_env.IsTest && _env.EnableMetrics)
            {
              PrometheusMetrics.SetStaticLabels(_env);
            }

            if (!_env.IsStaging)
            {
                app.UseClientRateLimiting();
            }

            if (_env.IsDevelopment)
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<SerilogCorrelationIdMiddleware>();
            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRequestResponseLogging();

            app.ConfigureHangfire(addBasicAuthFilter: !_env.IsDevelopment, _env);

            app.UseSwagger();

            if (!_env.IsProduction)
            {
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Get into Teaching API V1"));
            }

            app.UseRouting();

            if (_env.ExportHangireToPrometheus && _env.EnableMetrics)
            {
                app.UsePrometheusHangfireExporter();
            }

            if (_env.EnableMetrics)
            {
              app.UseHttpMetrics();
            }

            app.UseAuthorization();

            ResponseHeaders.SetupSecureHeaders(app);

            DatabaseUtility.Migrate(serviceScope, _env);

            if (!_env.IsTest)
            {
                HangfireJobs.AddCrmSyncJob();
                HangfireJobs.AddLocationSyncJob();
                HangfireJobs.AddMagicLinkTokenGenerationJob();

                if (_env.IsFeatureOn("APPLY_CANDIDATE_API"))
                {
                    HangfireJobs.AddApplySyncJob();
                } else
                {
                    HangfireJobs.RemoveApplySyncJob();
                }
            }

            if (!_env.IsTest)
            {
                DatabaseUtility.Seed(serviceScope);
                DatabaseUtility.SeedCountriesAndTeachingSubjects(serviceScope);
            }

            var clientPolicyStore = serviceScope.ServiceProvider.GetRequiredService<IClientPolicyStore>();
            clientPolicyStore.SeedAsync().GetAwaiter().GetResult();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapControllers();

                endpoints.MapGet("/healthcheck", async context => {
                    await context.Response.WriteAsync("OK");
                });
            });
        }
    }
}
