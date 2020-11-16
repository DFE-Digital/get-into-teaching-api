using System.Threading.Tasks;
using AspNetCoreRateLimit;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GetIntoTeachingApi
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateHostBuilder(args).Build();

            using var scope = webHost.Services.CreateScope();

            // Get the ClientPolicyStore instance.
            var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();

            // Seed client data from appsettings.
            await clientPolicyStore.SeedAsync();

            // Configure the database.
            var dbConfiguration = scope.ServiceProvider.GetRequiredService<DbConfiguration>();
            var env = scope.ServiceProvider.GetRequiredService<IEnv>();
            dbConfiguration.Configure(env.InstanceIndex);

            await webHost.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSentry();
                    webBuilder.UseStartup<Startup>();
                });
    }
}
