using Moq;
using Serilog.Core;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles
{
    public static class LogEventPropertyFactoryTestDouble
    {
        public static Mock<ILogEventPropertyFactory> Mock() => new();

        public static ILogEventPropertyFactory MockFor()
        {
            Mock<ILogEventPropertyFactory> logEventPropertyFactory = Mock();

            return logEventPropertyFactory.Object;
        }
    }
}
