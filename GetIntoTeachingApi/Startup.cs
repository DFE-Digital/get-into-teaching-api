using System;
using System.Linq;
using FluentValidation.AspNetCore;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Auth;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.OperationFilters;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Xrm.Sdk;
using Prometheus;
using Swashbuckle.AspNetCore.Swagger;

namespace GetIntoTeachingApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var env = new Env();

            services.AddSingleton<CdsServiceClientWrapper, CdsServiceClientWrapper>();
            services.AddTransient<IOrganizationService>(sp => sp.GetService<CdsServiceClientWrapper>().CdsServiceClient.Clone());
            services.AddTransient<IOrganizationServiceAdapter, OrganizationServiceAdapter>();
            services.AddTransient<ICrmService, CrmService>();

            services.AddScoped<IStore, Store>();
            services.AddScoped<DbConfiguration, DbConfiguration>();

            services.AddSingleton<IMetricService, MetricService>();
            services.AddSingleton<INotificationClientAdapter, NotificationClientAdapter>();
            services.AddSingleton<ICandidateAccessTokenService, CandidateAccessTokenService>();
            services.AddSingleton<INotifyService, NotifyService>();
            services.AddSingleton<IHangfireService, HangfireService>();
            services.AddSingleton<IPerformContextAdapter, PerformContextAdapter>();
            services.AddSingleton<IEnv>(env);

            if (env.IsDevelopment || env.IsTest)
            {
                var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
                services.AddDbContext<GetIntoTeachingDbContext>(builder => DbConfiguration.ConfigSqLite(builder, keepAliveConnection));
            }
            else
            {
                services.AddDbContext<GetIntoTeachingDbContext>(b => DbConfiguration.ConfigPostgres(env, b));
            }

            services.AddAuthentication("SharedSecretHandler")
                .AddScheme<SharedSecretSchemeOptions, SharedSecretHandler>("SharedSecretHandler", op => { });

            services.AddControllers().AddFluentValidation(c =>
            {
                c.RegisterValidatorsFromAssemblyContaining<Startup>();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Get into Teaching API - V1",
                        Version = "v1",
                        Description = @"
Provides a RESTful API for integrating with the Get into Teaching CRM.

The Get into Teaching (GIT) API sits in front of the GIT CRM, which uses the [Microsoft Dynamics365](https://docs.microsoft.com/en-us/dynamics365/) platform (the [Customer Engagement](https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/overview) module is used for storing Candidate information and the [Marketing](https://docs.microsoft.com/en-us/dynamics365/marketing/developer/using-events-api) module for managing Events).

The GIT API aims to provide:

* Simple, task-based RESTful APIs.
* Message queueing (while the GIT CRM is offline for updates).
* Validation to ensure consistency across services writing to the GIT CRM.
                        ",
                        License = new OpenApiLicense
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT"),
                        },
                    });

                c.AddSecurityDefinition("apiKey", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                });

                c.OperationFilter<AuthOperationFilter>();
                c.EnableAnnotations();
                c.AddFluentValidationRules();
            });

            services.AddHangfire((provider, config) =>
            {
                var automaticRetry = new AutomaticRetryAttribute
                {
                    Attempts = JobConfiguration.Attempts(env),
                    DelaysInSeconds = new[] { JobConfiguration.RetryIntervalInSeconds(env) },
                    OnAttemptsExceeded = AttemptsExceededAction.Delete,
                };

                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseFilter(automaticRetry);

                if (env.IsDevelopment || env.IsTest)
                {
                    config.UseMemoryStorage().WithJobExpirationTimeout(JobConfiguration.ExpirationTimeout);
                }
                else
                {
                    config.UsePostgreSqlStorage(DbConfiguration.HangfireConnectionString(env));
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment hostEnv)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var env = serviceScope.ServiceProvider.GetService<IEnv>();

            if (hostEnv.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();

            var hangfireOptions = new BackgroundJobServerOptions() { WorkerCount = 20 };
            app.UseHangfireServer(hangfireOptions);

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter(env) },
            });

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Get into Teaching API V1");
            });

            app.UseRouting();

            if (env.ExportHangireToPrometheus)
            {
                app.UsePrometheusHangfireExporter();
            }

            app.UseHttpMetrics();

            app.UseAuthorization();

            // Configure recurring jobs.
            RecurringJob.AddOrUpdate<CrmSyncJob>(JobConfiguration.CrmSyncJobId, (x) => x.RunAsync(), Cron.Daily);
            RecurringJob.AddOrUpdate<LocationSyncJob>(
                JobConfiguration.LocationSyncJobId,
                (x) => x.RunAsync("https://www.freemaptools.com/download/full-postcodes/ukpostcodes.zip"),
                Cron.Weekly());

            // Configure the database.
            var dbConfiguration = serviceScope.ServiceProvider.GetService<DbConfiguration>();
            dbConfiguration.Configure();

            // Don't seed test environment.
            if (!env.IsTest)
            {
                // Sync with the CRM.
                RecurringJob.Trigger(JobConfiguration.CrmSyncJobId);

                // Initial locations sync.
                var dbContext = serviceScope.ServiceProvider.GetService<GetIntoTeachingDbContext>();
                if (!dbContext.Locations.Any())
                {
                    RecurringJob.Trigger(JobConfiguration.LocationSyncJobId);
                }
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapControllers();
            });
        }
    }
}
