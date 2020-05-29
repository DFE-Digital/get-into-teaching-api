using System.Collections.Generic;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class TeachingEventRegistrationJobTests
    {
        private readonly Mock<IPerformContextAdapter> _mockContext;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly TeachingEventRegistration _registration;
        private readonly TeachingEventRegistrationJob _job;

        public TeachingEventRegistrationJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _registration = new TeachingEventRegistration() { CandidateEmail = "test@test.com" };
            _job = new TeachingEventRegistrationJob(_mockCrm.Object, _mockNotifyService.Object, _mockContext.Object);
        }

        [Fact]
        public void Run_OnSuccess_SavesCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_registration, null);

            _mockCrm.Verify(mock => mock.Save(_registration), Times.Once);
        }

        [Fact]
        public void Run_OnFailure_EmailsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(JobConfiguration.Attempts);

            _job.Run(_registration, null);

            _mockCrm.Verify(mock => mock.Save(_registration), Times.Never);
            _mockNotifyService.Verify(mock => mock.SendEmail(_registration.CandidateEmail,
                NotifyService.TeachingEventRegistrationFailedTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
        }
    }
}