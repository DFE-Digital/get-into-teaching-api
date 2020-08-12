using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class StatusCheckJobTests
    {
        [Fact]
        public void Run_LogsInfo()
        {
            var mockLogger = new Mock<ILogger<StatusCheckJob>>();
            var job = new StatusCheckJob(new Env(), mockLogger.Object);

            job.Run();

            mockLogger.VerifyInformationWasCalled("Hangfire - Status Check");
        }
    }
}