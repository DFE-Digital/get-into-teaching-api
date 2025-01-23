using GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers;
using GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles;
using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using Xunit;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers
{
    public sealed class CorrelationIdLogEnricherTests
    {
        [Fact]
        public void Enrich_WithNoFilterParam_AssignsCorrectPropertyValues()
        {
            // arrange
            var logEvent = new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Verbose, null,
                    new MessageTemplate("template", new List<MessageTemplateToken>()),
                    new List<LogEventProperty>
                    {
                        new LogEventProperty("name1", new ScalarValue("value")),
                        new LogEventProperty("name2", new ScalarValue(42))
                    });

            Guid correlationId = Guid.NewGuid();

            CorrelationIdLogEnricher enricher = new(
                httpContextAccessor: HttpContextAccessorTestDouble.DefaultMock(),
                httpContextCorrelationIdProvider: HttpContextCorrelationIdProviderTestDouble.MockFor(correlationId));

            // act
            enricher.Enrich(logEvent, propertyFactory: LogEventPropertyFactoryTestDouble.Mock());

            // assert
            //Assert.Equal(

        }
    }
}
