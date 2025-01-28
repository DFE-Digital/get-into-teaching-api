using FluentAssertions;
using GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers;
using GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles;
using Serilog.Events;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers
{
    public sealed class LogEventExtensionsTests
    {
        [Fact]
        public void LogProperty_WithSpecificPropertyKeyValue_SuccessfullyAddsToPropertyCollection()
        {
            // arrange
            LogEvent logEvent = LogEventTestDouble.LogEventStub();

            const string propertyKey = "Test-Key-1";
            const string propertyValue = "Test-Value-1";

            // act
            logEvent.LogProperty(
                propertyFactory: LogEventPropertyFactoryTestDouble.DefaultMock(),
                propertyKey,
                propertyValue);

            // assert
            Assert.NotNull(logEvent.Properties);
            Assert.True(logEvent.Properties.Count > 1);
            Assert.True(logEvent.Properties.ContainsKey(propertyKey));
        }

        [Fact]
        public void LogProperty_WithNullPropertyFactory_ThrowsExpectedException()
        {
            // arrange
            LogEvent logEvent = LogEventTestDouble.LogEventStub();

            const string propertyKey = "Test-Key-1";
            const string propertyValue = "Test-Value-1";

            // act, assert
            _ = (Assert.Throws<ArgumentNullException>(
                () => logEvent.LogProperty(propertyFactory: null!, propertyKey, propertyValue))
                    .Message.Should().Be("Value cannot be null. (Parameter 'propertyFactory')"));
        }

        [Fact]
        public void LogProperty_WithNullPropertyKey_ThrowsExpectedException()
        {
            // arrange
            LogEvent logEvent = LogEventTestDouble.LogEventStub();

            const string propertyValue = "Test-Value-1";

            // act, assert
            _ = (Assert.Throws<ArgumentNullException>(
                () => logEvent.LogProperty(
                    propertyFactory: LogEventPropertyFactoryTestDouble.DefaultMock(),
                    propertyKey: null!, propertyValue))
                    .Message.Should().Be("Value cannot be null. (Parameter 'propertyKey')"));
        }

        [Fact]
        public void LogProperty_WithNullPropertyValue_ThrowsExpectedException()
        {
            // arrange
            LogEvent logEvent = LogEventTestDouble.LogEventStub();

            const string propertyKey = "Test-Key-1";

            // act, assert
            _ = (Assert.Throws<ArgumentNullException>(
                () => logEvent.LogProperty(
                    propertyFactory: LogEventPropertyFactoryTestDouble.DefaultMock(),
                    propertyKey, propertyValue: null!))
                    .Message.Should().Be("Value cannot be null. (Parameter 'propertyValue')"));
        }
    }
}
