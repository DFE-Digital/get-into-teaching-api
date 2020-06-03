using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace GetIntoTeachingApiTests.Helpers
{
    public static class LoggerTestHelpers
    {
        public static Mock<ILogger<T>> VerifyWarningWasCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage)
        {
            Func<object, Type, bool> state = (v, t) =>  v.ToString().Contains(expectedMessage);

            logger.Verify(
                mock => mock.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => state(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
                )
            );

            return logger;
        }
    }
}
