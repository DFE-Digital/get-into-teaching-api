using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using GetIntoTeachingApi.Auth;
using System.Collections.Generic;
using GetIntoTeachingApi.OperationFilters;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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
            services.AddSingleton<IAuthorizationHandler, SharedSecretHandler>();
            services.AddSingleton<INotificationClientAdapter, NotificationClientAdapter>();
            services.AddSingleton<IOrganizationServiceAdapter, OrganizationServiceAdapter>();
            services.AddSingleton<ICandidateAccessTokenService, CandidateAccessTokenService>();
            services.AddScoped<ICrmService, CrmService>();
            services.AddSingleton<INotifyService, NotifyService>();
            services.AddSingleton<ICrmCache, CrmCache>();
            services.AddScoped<IStore, Store>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddSingleton<IPerformContextAdapter, PerformContextAdapter>();
            services.AddScoped<DbConfiguration, DbConfiguration>();

            if (Env.IsDevelopment)
            {
                var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
                keepAliveConnection.Open();
                services.AddDbContext<GetIntoTeachingDbContext>(options => options.UseSqlite(keepAliveConnection));
            }
            else
            {
                services.AddDbContext<GetIntoTeachingDbContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString(DbConfiguration.DatabaseConnectionString())));
            }

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SharedSecret", policy => 
                    policy.Requirements.Add(
                        new SharedSecretRequirement(Environment.GetEnvironmentVariable("SHARED_SECRET"))
                    )
                );
            });

            services.AddControllers().AddFluentValidation(c =>
            {
                c.RegisterValidatorsFromAssemblyContaining<Startup>();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
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
                            Url = new Uri("https://opensource.org/licenses/MIT")
                        }
                    }
                );

                c.AddSecurityDefinition("apiKey", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = ParameterLocation.Header
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "apiKey" }
                        },
                        new List<string>()
                    }
                });

                c.OperationFilter<AuthResponsesOperationFilter>();
                c.EnableAnnotations();
                c.AddFluentValidationRules();
            });

            services.AddHangfire((provider, config) =>
            {
                var automaticRetry = new AutomaticRetryAttribute
                {
                    Attempts = JobConfiguration.Attempts,
                    DelaysInSeconds = new[] { JobConfiguration.RetryIntervalInSeconds },
                    OnAttemptsExceeded = AttemptsExceededAction.Delete
                };

                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseFilter(automaticRetry);

                if (Env.IsDevelopment)
                    config.UseMemoryStorage().WithJobExpirationTimeout(JobConfiguration.ExpirationTimeout);
                else
                    config.UsePostgreSqlStorage(Configuration.GetConnectionString(DbConfiguration.HangfireConnectionString()));
            });

            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthroizationFilter() }
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Get into Teaching API V1");
            });

            app.UseAuthorization();

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                // Configure and seed the database.
                var dbConfiguration = serviceScope.ServiceProvider.GetService<DbConfiguration>();
                dbConfiguration.Configure();
            }

            // Kick off/update recurring jobs.
            RecurringJob.AddOrUpdate<CrmSyncJob>("crm-sync", (x) => x.Run(), Cron.Daily());
            RecurringJob.Trigger("crm-sync");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
