using System;
using System.Threading.Tasks;
using FluentAssertions;
using GetIntoTeachingApi.Services.Crm;
using GetIntoTeachingApiTests.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Services.Crm
{
    public class WebApiClientCacheTests
    {
        private readonly Mock<ILogger<WebApiClientCache>> _mockLogger;
        private readonly WebApiClientCache _cache;

        public WebApiClientCacheTests()
        {
            _mockLogger = new Mock<ILogger<WebApiClientCache>>();
            _cache = new WebApiClientCache(_mockLogger.Object);
        }

        [Fact]
        public async void GetOrCreate_WhenStoreIsEmpty_Creates()
        {
            var result = await _cache.GetOrCreateAsync("key", DateTime.Now.AddSeconds(30), () => Task.Run(() => "value"));

            result.Should().Be("value");
        }

        [Fact]
        public async void GetOrCreate_WhenEntryExpired_Refreshes()
        {
            await _cache.GetOrCreateAsync("key", DateTime.Now.AddSeconds(-30), () => Task.Run(() => "value"));

            var result = await _cache.GetOrCreateAsync("key", DateTime.Now.AddSeconds(30), () => Task.Run(() => "new-value"));

            result.Should().Be("new-value");
        }

        [Fact]
        public async void GetOrCreate_WhenEntryFresh_Gets()
        {
            await _cache.GetOrCreateAsync("key", DateTime.Now.AddSeconds(30), () => Task.Run(() => "value"));

            var result = await _cache.GetOrCreateAsync("key", DateTime.Now.AddSeconds(30), () => Task.Run(() => "new-value"));

            result.Should().Be("value");
        }

        [Fact]
        public async void GetOrCreate_WhenRefreshFails_ReturnsStaleData()
        {
            await _cache.GetOrCreateAsync("key", DateTime.Now.AddSeconds(-30), () => Task.Run(() => "value"));

            var result = await _cache.GetOrCreateAsync("key", DateTime.Now.AddSeconds(30), () =>
            {
                throw new Exception("bang");
#pragma warning disable 0162
                return Task.Run(() => "");
#pragma warning restore 0162
            });

            result.Should().Be("value");
            _mockLogger.VerifyWarningWasCalled("WebApiClientCache - Failed to refresh cache (key): bang");
        }
    }
}
