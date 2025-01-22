using Moq;
using Serilog.Core;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles
{
    public static class LogEventPropertyFactoryTestDouble
    {
        public static ILogEventPropertyFactory Mock()
        {
            Mock<ILogEventPropertyFactory> logEventPropertyFactory = new();

            return logEventPropertyFactory.Object;
        }
    }
}