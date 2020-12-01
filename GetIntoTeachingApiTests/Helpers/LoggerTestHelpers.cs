using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace GetIntoTeachingApiTests.Helpers
{
    public static class LoggerTestHelpers
    {
        public static Mock<ILogger<T>> VerifyInformationWasCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage)
        {
            return VerifyCalled(logger, LogLevel.Information, expectedMessage);
        }

        public static Mock<ILogger<T>> VerifyInformationWasCalledExactly<T>(this Mock<ILogger<T>> logger, string expectedMessage)
        {
            return VerifyCalledExactly(logger, LogLevel.Information, expectedMessage);
        }

        public static Mock<ILogger<T>> VerifyWarningWasCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage)
        {
            return VerifyCalled(logger, LogLevel.Warning, expectedMessage);
        }

        public static Mock<ILogger<T>> VerifyErrorWasCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage)
        {
            return VerifyCalled(logger, LogLevel.Error, expectedMessage);
        }

        private static Mock<ILogger<T>> VerifyCalledExactly<T>(this Mock<ILogger<T>> logger, LogLevel expectedLogLevel, string expectedMessage)
        {
            bool state(object v, Type t) => v.ToString() == expectedMessage;

            VerifyCalled(logger, expectedLogLevel, state);

            return logger;
        }

        private static Mock<ILogger<T>> VerifyCalled<T>(this Mock<ILogger<T>> logger, LogLevel expectedLogLevel, string expectedMessage)
        {
            bool state(object v, Type t) => v.ToString().Contains(expectedMessage);

            VerifyCalled(logger, expectedLogLevel, state);

            return logger;
        }

        private static void VerifyCalled<T>(this Mock<ILogger<T>> logger, LogLevel expectedLogLevel, Func<object, Type, bool> state)
        {
            logger.Verify(
                mock => mock.Log(
                    It.Is<LogLevel>(logLevel => logLevel == expectedLogLevel),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => state(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
                )
            );
        }
    }
}
