using FluentAssertions;
using GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers;
using GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.Extensions;
using GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles;
using GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.Shared.TestDoubles;
using Serilog.Events;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers
{
    public sealed class CorrelationIdLogEnricherTests
    {
        [Fact]
        public void Enrich_WithSingleLogEvent_AssignsCorrelationIdToLogEventPropertyValues()
        {
            // arrange
            LogEvent logEvent = LogEventTestDouble.LogEventStub();

            Guid correlationId = Guid.NewGuid();

            CorrelationIdLogEnricher enricher = new(
                httpContextAccessor: HttpContextAccessorTestDouble.DefaultMock(),
                httpContextCorrelationIdProvider: HttpContextCorrelationIdProviderTestDouble.MockFor(correlationId));

            // act
            enricher.Enrich(logEvent, propertyFactory: LogEventPropertyFactoryTestDouble.DefaultMock());

            // assert
            Assert.NotNull(logEvent.Properties);
            Assert.True(logEvent.Properties.Count > 1);
            Assert.True(logEvent.Properties.ContainsKey("CorrelationId"));
            Assert.Equivalent($"CID-{correlationId}", logEvent.GetScalarValue<string>("CorrelationId"));
        }

        [Fact]
        public void Enrich_WithMultipleLogEvents_AssignsCorrelationIdToLogEventPropertyValues()
        {
            // arrange
            Guid correlationId = Guid.NewGuid();

            LogEvent logEventFirst = LogEventTestDouble.LogEventStub();
            LogEvent logEventSecond = LogEventTestDouble.LogEventStub();

            CorrelationIdLogEnricher enricher = new(
                httpContextAccessor: HttpContextAccessorTestDouble.DefaultMock(),
                httpContextCorrelationIdProvider: HttpContextCorrelationIdProviderTestDouble.MockFor(correlationId));

            // act
            enricher.Enrich(logEventFirst, propertyFactory: LogEventPropertyFactoryTestDouble.DefaultMock());
            enricher.Enrich(logEventSecond, propertyFactory: LogEventPropertyFactoryTestDouble.DefaultMock());

            // assert
            Assert.NotNull(logEventFirst.Properties);
            Assert.True(logEventFirst.Properties.Count > 1);
            Assert.True(logEventFirst.Properties.ContainsKey("CorrelationId"));

            Assert.NotNull(logEventSecond.Properties);
            Assert.True(logEventSecond.Properties.Count > 1);
            Assert.True(logEventSecond.Properties.ContainsKey("CorrelationId"));

            Assert.Equivalent($"CID-{correlationId}", logEventFirst.GetScalarValue<string>("CorrelationId"));
            Assert.Equivalent($"CID-{correlationId}",logEventSecond.GetScalarValue<string>("CorrelationId"));
        }

        [Fact]
        public void Enrich_WithNullLogEvent_ThrowsExpectedException()
        {
            // arrange
            LogEvent logEvent = null;

            Guid correlationId = Guid.NewGuid();

            CorrelationIdLogEnricher enricher = new(
                httpContextAccessor: HttpContextAccessorTestDouble.DefaultMock(),
                httpContextCorrelationIdProvider: HttpContextCorrelationIdProviderTestDouble.MockFor(correlationId));

            // act, assert
            _ = (Assert.Throws<ArgumentNullException>(
                () => enricher.Enrich(logEvent, propertyFactory: LogEventPropertyFactoryTestDouble.DefaultMock()))
                    .Message.Should().Be("Value cannot be null. (Parameter 'logEvent')"));
        }

        [Fact]
        public void Enrich_WithNullLogEventPropertyFactory_ThrowsExpectedException()
        {
            // arrange
            LogEvent logEvent = LogEventTestDouble.LogEventStub();

            Guid correlationId = Guid.NewGuid();

            CorrelationIdLogEnricher enricher = new(
                httpContextAccessor: HttpContextAccessorTestDouble.DefaultMock(),
                httpContextCorrelationIdProvider: HttpContextCorrelationIdProviderTestDouble.MockFor(correlationId));

            // act, assert
            _ = (Assert.Throws<ArgumentNullException>(
                () => enricher.Enrich(logEvent, propertyFactory: null!))
                    .Message.Should().Be("Value cannot be null. (Parameter 'propertyFactory')"));
        }

        [Fact]
        public void Enrich_WithNullPropertyValue_ThrowsExpectedException()
        {
            // arrange
            LogEvent logEvent = LogEventTestDouble.LogEventStub();

            Guid correlationId = Guid.NewGuid();

            CorrelationIdLogEnricher enricher = new(
                httpContextAccessor: HttpContextAccessorTestDouble.MockFor(
                    HttpContextTestDouble.Mock().WithSpecificRequestPath(null!).Object),
                httpContextCorrelationIdProvider: HttpContextCorrelationIdProviderTestDouble.MockFor(correlationId));

            // act, assert
            _ = (Assert.Throws<ArgumentNullException>(
                () => enricher.Enrich(logEvent, propertyFactory: LogEventPropertyFactoryTestDouble.DefaultMock()))
                    .Message.Should().Be("Value cannot be null. (Parameter 'propertyValue')"));
        }
    }
}
