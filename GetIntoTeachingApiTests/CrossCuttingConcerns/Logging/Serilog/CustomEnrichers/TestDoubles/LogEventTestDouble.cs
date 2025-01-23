using Serilog.Events;
using System.Collections.Generic;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles
{
    public static class LogEventTestDouble
    {
        public static class LogEventPropertyStub
        {
            public static LogEvent LogEventStub() =>
                new LogEvent(
                    timestamp: new Bogus.Faker().Date.FutureOffset(),
                    level: (LogEventLevel)new Bogus.Faker().Random.Number(0, 5),
                    exception: null,
                    messageTemplate: )

            

            //var logEvent = new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Verbose, null,
            //new MessageTemplate("atemplate", new List<MessageTemplateToken>()),
            //new List<LogEventProperty>
            //{
            //                new LogEventProperty("aname1", new ScalarValue("avalue")),
            //                new LogEventProperty("aname2", new ScalarValue(42))
            //});
        }

        internal static class MessageTemplate
        {
        
        }

        internal static class LogEventPropertyTestDouble
        {
            public static IList<LogEventProperty> CreateLogEventsStub()
            {
                int propertyCount = new Bogus.Faker().Random.Number(1, 5);
                IList<LogEventProperty> logEventProperties = new List<LogEventProperty>();

                for (int i = 0; i < propertyCount; i++)
                {
                    logEventProperties.Add(CreateLogEventPropertyStub());
                }

                return logEventProperties;
            }

            public static LogEventProperty CreateLogEventPropertyStub()
            {
                var faker = new Bogus.Faker();
                return new LogEventProperty(
                    faker.Random.Word(),
                    new ScalarValue(faker.Random.Word()));
            }
        }
    }
}
