using System.Linq;
using AspNetCoreRateLimit;
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
            builder.ConfigureServices(services =>
            {
                // Use in-memory rate limit counters for tests.
                services.Remove(services.First(d => d.ServiceType == typeof(IRateLimitCounterStore)));
                var descriptor = new ServiceDescriptor(typeof(IRateLimitCounterStore), typeof(MemoryCacheRateLimitCounterStore), ServiceLifetime.Singleton);
                services.Add(descriptor);
            });
        }
    }
}
