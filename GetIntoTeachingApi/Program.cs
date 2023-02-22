using System;
using System.Threading.Tasks;
using GetIntoTeachingApi.AppStart;
using GetIntoTeachingApi.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GetIntoTeachingApi
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // Set global Regex timeout.
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(500));

            GetIntoTeachingDbContext.ConfigureNpgsql();

            var webHost = CreateHostBuilder(args).Build();

            await webHost.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider((_, options) => options.ValidateScopes = false)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSentry();
                    webBuilder.UseKestrel(opts => opts.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                })
            .UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration));
    }
}