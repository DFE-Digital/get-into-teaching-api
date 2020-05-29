using System.Collections.Generic;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
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

        public CandidateRegistrationJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _candidate = new Candidate() { Email = "test@test.com" };
            _job = new CandidateRegistrationJob(_mockCrm.Object, _mockNotifyService.Object, _mockContext.Object);
        }

        [Fact]
        public void Run_OnSuccess_SavesCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(_candidate), Times.Once);
        }

        [Fact]
        public void Run_OnFailure_EmailsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(JobConfiguration.Attempts);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(_candidate), Times.Never);
            _mockNotifyService.Verify(mock => mock.SendEmail(_candidate.Email, 
                NotifyService.CandidateRegistrationFailedTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
        }
    }
}
