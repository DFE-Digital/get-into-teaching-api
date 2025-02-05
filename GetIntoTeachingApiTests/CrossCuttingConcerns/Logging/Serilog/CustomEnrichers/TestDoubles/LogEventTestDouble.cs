using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.IO;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles
{
    public static class LogEventTestDouble
    {
        public static LogEvent LogEventStub() =>
            new(
                timestamp: new Bogus.Faker().Date.FutureOffset(),
                level: (LogEventLevel)new Bogus.Faker().Random.Number(0, 5),
                exception: null,
                messageTemplate: MessageTemplateTestDouble.CreateMessageTemplateStub(),
                properties: LogEventPropertyTestDouble.CreateLogEventsStub());

        internal static class MessageTemplateTestDouble
        {
            private static readonly Bogus.Faker faker = new();

            public static MessageTemplate CreateMessageTemplateStub()
            {
                return new MessageTemplate(
                    text: faker.Random.Word(),
                    tokens: CreateMessageTemplateTokensStub());
            }

            public static IList<MessageTemplateToken> CreateMessageTemplateTokensStub()
            {
                int tokenCount = faker.Random.Number(1, 10);
                IList<MessageTemplateToken> messageTemplateTokens = new List<MessageTemplateToken>();

                for (int i = 0; i < tokenCount; i++)
                {
                    messageTemplateTokens.Add(new TestMessageTemplateToken());
                }

                return messageTemplateTokens;
            }

            internal class TestMessageTemplateToken : MessageTemplateToken
            {
                public override int Length => faker.Random.Number(1, 10);

                public override void Render(
                    IReadOnlyDictionary<string, LogEventPropertyValue> properties,
                    TextWriter output, IFormatProvider formatProvider = null)
                {
                    throw new NotImplementedException();
                }
            }
        }

        internal static class LogEventPropertyTestDouble
        {
            private static readonly Bogus.Faker faker = new();

            public static IList<LogEventProperty> CreateLogEventsStub()
            {
                int propertyCount = faker.Random.Number(1, 5);
                IList<LogEventProperty> logEventProperties = new List<LogEventProperty>();

                for (int i = 0; i < propertyCount; i++)
                {
                    logEventProperties.Add(CreateLogEventPropertyStub());
                }

                return logEventProperties;
            }

            public static LogEventProperty CreateLogEventPropertyStub() =>
                new(
                    name: faker.Random.Word(),
                    value: new ScalarValue(faker.Random.Word()));
        }
    }
}
