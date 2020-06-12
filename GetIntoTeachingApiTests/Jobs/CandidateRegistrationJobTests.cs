using System.Collections.Generic;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class CandidateRegistrationJobTests
    {
        private readonly Mock<IPerformContextAdapter> _mockContext;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Candidate _candidate;
        private readonly CandidateRegistrationJob _job;
        private readonly Mock<ILogger<CandidateRegistrationJob>> _mockLogger;

        public CandidateRegistrationJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _mockLogger = new Mock<ILogger<CandidateRegistrationJob>>();
            _candidate = new Candidate() { Email = "test@test.com" };
            _job = new CandidateRegistrationJob(_mockCrm.Object, _mockNotifyService.Object,
                _mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public void Run_OnSuccess_SavesCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(_candidate), Times.Once);
            _mockLogger.VerifyInformationWasCalled("CandidateRegistrationJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled("CandidateRegistrationJob - Succeeded");
        }

        [Fact]
        public void Run_OnFailure_EmailsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(JobConfiguration.Attempts - 1);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(_candidate), Times.Never);
            _mockNotifyService.Verify(mock => mock.SendEmailAsync(_candidate.Email, 
                NotifyService.CandidateRegistrationFailedTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
            _mockLogger.VerifyInformationWasCalled("CandidateRegistrationJob - Started (24/24)");
            _mockLogger.VerifyInformationWasCalled("CandidateRegistrationJob - Deleted");
        }
    }
}
