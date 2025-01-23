using GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers;
using GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles;
using Serilog.Events;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers
{
    public sealed class CorrelationIdLogEnricherTests
    {
        [Fact]
        public void Enrich_WithNoFilterParam_AssignsCorrectPropertyValues()
        {
            // arrange
            LogEvent logEvent = LogEventTestDouble.LogEventStub();

            Guid correlationId = Guid.NewGuid();

            CorrelationIdLogEnricher enricher = new(
                httpContextAccessor: HttpContextAccessorTestDouble.DefaultMock(),
                httpContextCorrelationIdProvider: HttpContextCorrelationIdProviderTestDouble.MockFor(correlationId));

            // act
            enricher.Enrich(logEvent, propertyFactory: LogEventPropertyFactoryTestDouble.MockFor());

            // assert
            //Assert.Equal(

        }
    }
}
