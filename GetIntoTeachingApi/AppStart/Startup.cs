using System.Text.Json.Serialization;
using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using GetIntoTeachingApi.AppStart.Extensions;
using GetIntoTeachingApi.Auth;
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

        public virtual void Configure(IApplicationBuilder app)
        {
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapControllers();
            });
        }

        protected IServiceScope CreateScope(IApplicationBuilder app)
        {
            return app.ApplicationServices.CreateScope();
        }

        protected void ConfigureRateLimiting(IServiceScope scope)
        {
            var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();
            clientPolicyStore.SeedAsync().GetAwaiter().GetResult();
        }
    }
}
