using System;
using System.Linq;
using AspNetCoreRateLimit;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GetIntoTeachingApiTests.Helpers
{
    public class ContractTestGitWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        public readonly ContractTestOrganizationServiceAdapter ContractOrganizationServiceAdapter;
        public readonly ContractCallbackBookingService ContractCallbackBookingService;
        private readonly ContractTestState _state;

        public ContractTestGitWebApplicationFactory(ContractTestState state)
        {
            ContractOrganizationServiceAdapter = new ContractTestOrganizationServiceAdapter();
            ContractCallbackBookingService = new ContractCallbackBookingService();
            _state = state;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Use in-memory rate limit counters for tests.
                services.Remove(services.First(d => d.ServiceType == typeof(IRateLimitCounterStore)));
                services.Add(ServiceDescriptor.Describe(typeof(IRateLimitCounterStore), typeof(MemoryCacheRateLimitCounterStore), ServiceLifetime.Singleton));

                // Write track entities instead of calling API.
                services.Remove(services.First(d => d.ServiceType == typeof(IOrganizationServiceAdapter)));
                services.Add(ServiceDescriptor.Describe(typeof(IOrganizationServiceAdapter), provider => ContractOrganizationServiceAdapter, ServiceLifetime.Singleton));

                // Return mock quotas instead of calling API.
                services.Remove(services.First(d => d.ServiceType == typeof(ICallbackBookingService)));
                services.Add(ServiceDescriptor.Describe(typeof(ICallbackBookingService), provider => ContractCallbackBookingService, ServiceLifetime.Singleton));

                // Freeze date/time.
                services.Remove(services.First(d => d.ServiceType == typeof(IDateTimeProvider)));
                services.Add(ServiceDescriptor.Describe(typeof(IDateTimeProvider), provider => new FrozenDateTimeProvider(_state.UtcNow), ServiceLifetime.Singleton));
            });
        }

        private class FrozenDateTimeProvider : IDateTimeProvider
        {
            private readonly DateTime _utcNow;

            public FrozenDateTimeProvider(DateTime utcNow)
            {
                _utcNow = utcNow;
            }

            public DateTime UtcNow
            {
                get
                {
                    return _utcNow;
                }
            }
        }
    }
}
