using System;
using FluentAssertions;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class CrmCacheTests
    {
        private readonly Mock<ILogger<CrmCache>> _mockLogger;
        private readonly CrmCache _cache;

        public CrmCacheTests()
        {
            _mockLogger = new Mock<ILogger<CrmCache>>();
            _cache = new CrmCache(_mockLogger.Object);
        }

        [Fact]
        public void GetOrCreate_WhenStoreIsEmpty_Creates()
        {
            var result = _cache.GetOrCreate("key", DateTime.Now.AddSeconds(30), () => "value");

            result.Should().Be("value");
        }

        [Fact]
        public void GetOrCreate_WhenEntryExpired_Refreshes()
        {
            _cache.GetOrCreate("key", DateTime.Now.AddSeconds(-30), () => "value");

            var result = _cache.GetOrCreate("key", DateTime.Now.AddSeconds(30), () => "new-value");

            result.Should().Be("new-value");
        }

        [Fact]
        public void GetOrCreate_WhenEntryFresh_Gets()
        {
            _cache.GetOrCreate("key", DateTime.Now.AddSeconds(30), () => "value");

            var result = _cache.GetOrCreate("key", DateTime.Now.AddSeconds(30), () => "new-value");

            result.Should().Be("value");
        }

        [Fact]
        public void GetOrCreate_WhenRefreshFails_ReturnsStaleData()
        {
            _cache.GetOrCreate("key", DateTime.Now.AddSeconds(-30), () => "value");

            var result = _cache.GetOrCreate("key", DateTime.Now.AddSeconds(30), () =>
            { 
                throw new Exception("bang");
                #pragma warning disable 0162
                return "";
                #pragma warning restore 0162
            });

            result.Should().Be("value");
            _mockLogger.VerifyWarningWasCalled("CrmCache - Failed to refresh cache (key): bang");
        }
    }
}
