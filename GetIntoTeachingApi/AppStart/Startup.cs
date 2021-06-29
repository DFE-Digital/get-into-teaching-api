using System.Linq;
using System.Text.Json.Serialization;
using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using GetIntoTeachingApi.AppStart.Extensions;
using GetIntoTeachingApi.Auth;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.JsonConverters;
using GetIntoTeachingApi.ModelBinders;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace GetIntoTeachingApi.AppStart
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Env = new Env();
        }

        public IConfiguration Configuration { get; }
        protected IEnv Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.RegisterServices(Configuration, Env);

            services.AddDatabase(Env);

            services.AddApiAuthentication();

            services.AddMvc(o => o.Conventions.Add(new CommaSeparatedQueryStringConvention()));

            services.AddControllers(o => o.ModelBinderProviders.Insert(0, new TrimStringModelBinderProvider()))
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

            services.AddApiHangfire(Env);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRequestResponseLogging();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter(Env) },
            });

            // The UI generated for V2 is buggy, so use V3 in development.
            app.UseSwagger(c => c.SerializeAsV2 = !Env.IsDevelopment);

            app.UseRouting();

            if (Env.ExportHangireToPrometheus)
            {
                app.UsePrometheusHangfireExporter();
            }

            app.UseHttpMetrics();

            app.UseAuthorization();

            // Configure the database.
            var dbConfiguration = serviceScope.ServiceProvider.GetRequiredService<DbConfiguration>();

            if (Env.IsMasterInstance)
            {
                dbConfiguration.Migrate();
            }

            // Don't seed test environment.
            if (!Env.IsTest)
            {
                var dbContext = serviceScope.ServiceProvider.GetService<GetIntoTeachingDbContext>();

                // Initial CRM sync.
                if (!dbContext.PickListItems.Any())
                {
                    RecurringJob.Trigger(JobConfiguration.CrmSyncJobId);
                }

                // Initial locations sync.s
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
    }
}
