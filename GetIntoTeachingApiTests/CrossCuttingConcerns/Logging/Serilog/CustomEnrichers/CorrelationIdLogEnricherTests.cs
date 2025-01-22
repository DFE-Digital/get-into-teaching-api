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
                    new MessageTemplate("atemplate", new List<MessageTemplateToken>()),
                    new List<LogEventProperty>
                    {
                        new LogEventProperty("aname1", new ScalarValue("avalue")),
                        new LogEventProperty("aname2", new ScalarValue(42))
                    });

            Guid correlationId = Guid.NewGuid();

            CorrelationIdLogEnricher enricher = new(
                httpContextAccessor: HttpContextAccessorTestDouble.MockFor(context: ),
                httpContextCorrelationIdProvider: HttpContextCorrelationIdProviderTestDouble.MockFor(correlationId));

            // act
            enricher.Enrich(logEvent, propertyFactory: LogEventPropertyFactoryTestDouble.Mock());

            // assert
            Assert.Equal(2, logEvent.Properties.Count);
            Assert.Equal("\"avalue\"", logEvent.Properties["aname1"].ToString());
            Assert.Equal("42", logEvent.Properties["aname2"].ToString());
        }
    }
}
