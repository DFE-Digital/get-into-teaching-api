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
using GetIntoTeachingApi.Requirements;
using System.Collections.Generic;
using GetIntoTeachingApi.OperationFilters;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Services;
using AutoMapper;

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
            services.AddAutoMapper(typeof(Startup));

            services.AddSingleton<IAuthorizationHandler, SharedSecretHandler>();
            services.AddSingleton<INotificationClientAdapter, NotificationClientAdapter>();
            services.AddSingleton<IOrganizationServiceContextAdapter, OrganizationServiceContextAdapter>();
            services.AddSingleton<ICandidateAccessTokenService, CandidateAccessTokenService>();
            services.AddSingleton<ICrmService, CrmService>();
            services.AddSingleton<INotifyService, NotifyService>();

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

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Get into Teaching API V1");
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
