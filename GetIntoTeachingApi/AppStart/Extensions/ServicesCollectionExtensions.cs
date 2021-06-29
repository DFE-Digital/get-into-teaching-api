using System;
using AspNetCoreRateLimit;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Auth;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Middleware;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.OperationFilters;
using GetIntoTeachingApi.RateLimiting;
using GetIntoTeachingApi.Redis;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.Xrm.Sdk;
using Swashbuckle.AspNetCore.Swagger;

namespace GetIntoTeachingApi.AppStart.Extensions
{
    public static class ServicesCollectionExtensions
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration, IEnv env)
        {
            ConfigureRateLimiting(services, configuration);

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
            services.AddSingleton(env);
            services.AddSingleton<IRequestResponseLoggingConfiguration, RequestResponseLoggingConfiguration>();
        }

        public static void AddDatabase(this IServiceCollection services, IEnv env)
        {
            var connectionString = DbConfiguration.DatabaseConnectionString(env);
            services.AddDbContext<GetIntoTeachingDbContext>(b => DbConfiguration.ConfigPostgres(connectionString, b));
        }

        public static void AddApiAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication("ApiClientHandler")
               .AddScheme<ApiClientSchemaOptions, ApiClientHandler>("ApiClientHandler", _ => { });
        }

        public static void AddSwagger(this IServiceCollection services)
        {
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
        }

        public static void AddApiHangfire(this IServiceCollection services, IEnv env)
        {
            services.AddHangfire((_, config) =>
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

        private static void ConfigureRateLimiting(IServiceCollection services, IConfiguration configuration)
        {
            // Load appsettings.json.
            services.AddOptions();

            // Stores counters/IP rules.
            services.AddMemoryCache();

            // Load configuration/client settings from appsettings.json
            services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));
            services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));

            // Inject counter and rules stores.
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, RedisRateLimitCounterStore>();

            // https://github.com/aspnet/Hosting/issues/793
            // The IHttpContextAccessor service is not registered by default.
            // The clientId/clientIp resolvers use it.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Configuration (resolvers, counter key builders).
            services.AddSingleton<IRateLimitConfiguration, ApiClientRateLimitConfiguration>();
        }
    }
}
