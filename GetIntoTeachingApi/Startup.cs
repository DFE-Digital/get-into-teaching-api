using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AspNetCoreRateLimit;
using AspNetCoreRateLimit.Redis;
using dotenv.net;
using FluentValidation.AspNetCore;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Auth;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.JsonConverters;
using GetIntoTeachingApi.Middleware;
using GetIntoTeachingApi.ModelBinders;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.OperationFilters;
using GetIntoTeachingApi.RateLimiting;
using GetIntoTeachingApi.Redis;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using HangfireBasicAuthenticationFilter;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Xrm.Sdk;
using Prometheus;
using StackExchange.Redis;

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

            ConfigureRateLimiting(services);

            if (env.IsDevelopment)
            {
                DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false, envFilePaths: new[] { ".env.development" }));
            }

            services.AddSingleton<IAppSettings, AppSettings>();

            services.AddSingleton<CdsServiceClientWrapper, CdsServiceClientWrapper>();
            services.AddTransient<IOrganizationService>(sp => sp.GetService<CdsServiceClientWrapper>().CdsServiceClient?.Clone());
            services.AddTransient<IOrganizationServiceAdapter, OrganizationServiceAdapter>();

            services.AddTransient<ICrmService, CrmService>();

            services.AddScoped<IStore, Store>();
            services.AddScoped<DbConfiguration, DbConfiguration>();

            services.AddSingleton<IMetricService, MetricService>();
            services.AddSingleton<INotificationClientAdapter, NotificationClientAdapter>();
            services.AddSingleton<IGeocodeClientAdapter, GeocodeClientAdapter>();
            services.AddSingleton<ICandidateAccessTokenService, CandidateAccessTokenService>();
            services.AddSingleton<ICandidateMagicLinkTokenService, CandidateMagicLinkTokenService>();
            services.AddSingleton<INotifyService, NotifyService>();
            services.AddSingleton<IClientManager, ClientManager>();
            services.AddSingleton<IHangfireService, HangfireService>();
            services.AddSingleton<IRedisService, RedisService>();
            services.AddSingleton<ICandidateUpserter, CandidateUpserter>();
            services.AddSingleton<IPerformContextAdapter, PerformContextAdapter>();
            services.AddSingleton<ICallbackBookingService, CallbackBookingService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IEnv>(env);
            services.AddSingleton<IRequestResponseLoggingConfiguration, RequestResponseLoggingConfiguration>();

            var redisOptions = RedisConfiguration.ConfigurationOptions(env);
            services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect(redisOptions));

            var connectionString = DbConfiguration.DatabaseConnectionString(env);
            services.AddDbContext<GetIntoTeachingDbContext>(b => DbConfiguration.ConfigPostgres(connectionString, b));

            services.AddAuthentication("ApiClientHandler")
                .AddScheme<ApiClientSchemaOptions, ApiClientHandler>("ApiClientHandler", op => { });

            services.AddDataProtection().PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(redisOptions), "DataProtection-Keys");

            services.AddMvc(o =>
            {
                o.Conventions.Add(new CommaSeparatedQueryStringConvention());
            });

            services.AddControllers(o =>
            {
                o.ModelBinderProviders.Insert(0, new TrimStringModelBinderProvider());
            })
            .AddFluentValidation(c =>
            {
                c.RegisterValidatorsFromAssemblyContaining<Startup>();
            })
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
            });

            services.AddFluentValidationRulesToSwagger();

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

                if (env.IsTest)
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

            // We can't do this in test as the DefaultRegistry is shared between runs
            // and we get an error trying to set static labels once metrics have been registed.
            // There doesn't appear to be a way to clear the DefaultRegistry between tests.
            if (!env.IsTest && !Metrics.DefaultRegistry.StaticLabels.Any())
            {
                Metrics.DefaultRegistry.SetStaticLabels(new Dictionary<string, string>
                {
                    { "app", env.AppName },
                    { "organization", env.Organization },
                    { "space", env.Space },
                });
            }

            if (!env.IsStaging)
            {
                app.UseClientRateLimiting();
            }

            if (hostEnv.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRequestResponseLogging();

            ConfigureHangfire(app, env);

            app.UseSwagger(c =>
            {
                // The UI generated for V2 is buggy, so use V3 in development.
                c.SerializeAsV2 = !env.IsDevelopment;
            });

            if (!env.IsProduction)
            {
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Get into Teaching API V1");
                });
            }

            app.UseRouting();

            if (env.ExportHangireToPrometheus)
            {
                app.UsePrometheusHangfireExporter();
            }

            app.UseHttpMetrics();

            app.UseAuthorization();

            // Configure the database.
            var dbConfiguration = serviceScope.ServiceProvider.GetRequiredService<DbConfiguration>();

            if (env.IsMasterInstance)
            {
                dbConfiguration.Migrate();
            }

            // Don't run recurring jobs in test environment.
            if (!env.IsTest)
            {
                // Configure recurring jobs.
                const string everyFifthMinute = "*/5 * * * *";
                RecurringJob.AddOrUpdate<CrmSyncJob>(JobConfiguration.CrmSyncJobId, (x) => x.RunAsync(), everyFifthMinute);
                RecurringJob.AddOrUpdate<LocationSyncJob>(
                    JobConfiguration.LocationSyncJobId,
                    (x) => x.RunAsync(LocationSyncJob.FreeMapToolsUrl),
                    Cron.Weekly());
                RecurringJob.AddOrUpdate<MagicLinkTokenGenerationJob>(
                    JobConfiguration.MagicLinkTokenGenerationJobId,
                    (x) => x.Run(),
                    Cron.Hourly());

                // Only run FindApplySyncJob in dev/staging environments for now.
                if (!env.IsProduction)
                {
                    RecurringJob.AddOrUpdate<FindApplySyncJob>(
                        JobConfiguration.FindApplySyncJobId,
                        (x) => x.RunAsync(),
                        Cron.Hourly());
                }
            }

            // Don't seed test environment.
            if (!env.IsTest)
            {
                var dbContext = serviceScope.ServiceProvider.GetService<GetIntoTeachingDbContext>();

                // Initial CRM sync.
                if (!dbContext.PickListItems.Any())
                {
                    RecurringJob.Trigger(JobConfiguration.CrmSyncJobId);
                }

                // Initial locations sync.
                if (!dbContext.Locations.Any())
                {
                    RecurringJob.Trigger(JobConfiguration.LocationSyncJobId);
                }
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

        private static void ConfigureHangfire(IApplicationBuilder app, IEnv env)
        {
            var hangfireOptions = new BackgroundJobServerOptions();
            if (!env.IsDevelopment)
            {
                hangfireOptions.WorkerCount = 20;
            }

            app.UseHangfireServer(hangfireOptions);

            var filters = new List<IDashboardAuthorizationFilter> { new HangfireDashboardEnvironmentAuthorizationFilter(env) };

            if (env.IsStaging)
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

        private void ConfigureRateLimiting(IServiceCollection services)
        {
            // Load appsettings.json.
            services.AddOptions();

            // Stores counters/IP rules.
            services.AddMemoryCache();

            // Load configuration/client settings from appsettings.json
            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));

            // Setup Redis.
            services.AddDistributedRateLimiting<RedisProcessingStrategy>();
            services.AddRedisRateLimiting();

            // Configuration (resolvers, counter key builders).
            services.AddSingleton<IRateLimitConfiguration, ApiClientRateLimitConfiguration>();
        }
    }
}
