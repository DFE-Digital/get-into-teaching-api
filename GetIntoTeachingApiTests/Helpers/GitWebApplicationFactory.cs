using System.Linq;
using AspNetCoreRateLimit;
using GetIntoTeachingApi.Adapters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GetIntoTeachingApiTests.Helpers
{
    public class GitWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSentry(dsn: "");
            
            builder.ConfigureServices(services =>
            {
                // Remove Redis rate limiting.
                services.Remove(services.First(d => d.ServiceType == typeof(IRateLimitCounterStore)));
                services.Remove(services.First(d => d.ServiceType == typeof(IClientPolicyStore)));
                services.Remove(services.First(d => d.ServiceType == typeof(IProcessingStrategy)));

                // Use in-memory rate limiting.
                services.AddInMemoryRateLimiting();

                // Mock out API calls.
                services.Remove(services.First(d => d.ServiceType == typeof(IOrganizationServiceAdapter)));
                services.Add(ServiceDescriptor.Describe(typeof(IOrganizationServiceAdapter), provider => new MockOrganizationServiceAdapter(), ServiceLifetime.Singleton));
            });
        }
    }
}
