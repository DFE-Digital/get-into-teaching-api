using GetIntoTeachingApi.AppStart;
using GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers;
using GetIntoTeachingApi.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace GetIntoTeachingApi
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // Set global Regex timeout.
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(500));

            GetIntoTeachingDbContext.ConfigureNpgsql();

            Log.Logger = new LoggerConfiguration()
              .WriteTo.Sentry(o => o.Dsn = "https://77e5a366d39a433cbea90a992edab82c@o225781.ingest.us.sentry.io/5276954")
              .CreateLogger();

            var webHost = CreateHostBuilder(args).Build();

            await webHost.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider((_, options) => options.ValidateScopes = false)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseSentry();
                    //webBuilder.UseKestrel(opts => opts.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureServices(services =>
                    {
                        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                        services.AddSingleton<CorrelationIdLogEnricher>();
                    });
                })
                .UseSerilog((ctx, serviceProvider, config) =>
                    config.ReadFrom.Configuration(ctx.Configuration)
                        .Enrich.With(serviceProvider.GetRequiredService<CorrelationIdLogEnricher>())
                        .WriteTo.Sentry(s =>
                        {
                            s.Dsn = "https://77e5a366d39a433cbea90a992edab82c@o225781.ingest.us.sentry.io/5276954";  // new Dsn(ConfigurationManager.AppSettings["SentryDsn"]);
                            s.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                            s.MinimumEventLevel = LogEventLevel.Error;
                        }));
    }
}