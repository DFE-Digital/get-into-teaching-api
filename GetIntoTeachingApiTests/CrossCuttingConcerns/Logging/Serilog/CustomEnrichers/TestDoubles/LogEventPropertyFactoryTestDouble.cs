using Moq;
using Serilog.Core;
using Serilog.Events;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles
{
    public static class LogEventPropertyFactoryTestDouble
    {
        public static Mock<ILogEventPropertyFactory> Mock() => new();

        public static ILogEventPropertyFactory DefaultMock(string propertyKey = null, object propertyValue = null)
        {
            Mock<ILogEventPropertyFactory> logEventPropertyFactory = Mock();

            logEventPropertyFactory.Setup(factory =>
                factory.CreateProperty(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
                    .Returns((string propertyKey, object propertyValue, bool destructorObjects) =>
                        new LogEventProperty(propertyKey, new ScalarValue(propertyValue)));


            return logEventPropertyFactory.Object;
        }
    }
}
