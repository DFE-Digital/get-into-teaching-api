using System;
using System.Configuration;
using System.Threading.Tasks;
using GetIntoTeachingApi.AppStart;
using GetIntoTeachingApi.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

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
                })
            .UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration).WriteTo.Sentry(s =>
                {
                    s.Dsn = "https://77e5a366d39a433cbea90a992edab82c@o225781.ingest.us.sentry.io/5276954";  // new Dsn(ConfigurationManager.AppSettings["SentryDsn"]);
                    s.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                    s.MinimumEventLevel = LogEventLevel.Error;
                }));


        //.UseSerilog((_, c) =>
        //    c.Enrich.FromLogContext()
        //        .MinimumLevel.Debug()
        //        .WriteTo.Console()
        //        // Add Sentry integration with Serilog
        //        // Two levels are used to configure it.
        //        // One sets which log level is minimally required to keep a log message as breadcrumbs
        //        // The other sets the minimum level for messages to be sent out as events to Sentry
        //        .WriteTo.Sentry(s =>
        //        {
        //            s.MinimumBreadcrumbLevel = LogEventLevel.Debug;
        //            s.MinimumEventLevel = LogEventLevel.Error;
        //        }));


    }
}