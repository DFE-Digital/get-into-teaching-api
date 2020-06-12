using System;
using System.Collections.Generic;
using Castle.Core.Logging;
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
    public class TeachingEventRegistrationJobTests
    {
        private readonly Mock<IPerformContextAdapter> _mockContext;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Mock<ILogger<TeachingEventRegistrationJob>> _mockLogger;
        private readonly ExistingCandidateRequest _attendee;
        private readonly Guid _teachingEventId;
        private readonly TeachingEventRegistrationJob _job;

        public TeachingEventRegistrationJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _teachingEventId = Guid.NewGuid();
            _mockLogger = new Mock<ILogger<TeachingEventRegistrationJob>>();
            _attendee = new ExistingCandidateRequest() { Email = "test@test.com", FirstName = "first", LastName = "last" };
            _job = new TeachingEventRegistrationJob(_mockCrm.Object, _mockNotifyService.Object, 
                _mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public void Run_OnSuccessWithExistingCandidate_SavesRegistration()
        {
            var candidate = new Candidate() { Id = Guid.NewGuid() };
            _mockCrm.Setup(m => m.GetCandidate(_attendee)).Returns(candidate);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_attendee, _teachingEventId, null);

            _mockCrm.Verify(mock => mock.Save(It.Is<TeachingEventRegistration>(r =>
                r.EventId == _teachingEventId &&
                r.CandidateId == candidate.Id)), Times.Once);
            _mockLogger.VerifyInformationWasCalled("TeachingEventRegistrationJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled("TeachingEventRegistrationJob - Succeeded");
        }

        [Fact]
        public void Run_OnSuccessWithNewCandidate_SavesRegistration()
        {
            var candidateId = Guid.NewGuid();
            _mockCrm.Setup(m => m.GetCandidate(_attendee)).Returns<Candidate>(null);
            _mockCrm.Setup(m => m.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_attendee, _teachingEventId, null);

            _mockCrm.Verify(mock => mock.Save(It.Is<TeachingEventRegistration>(r => 
                r.EventId == _teachingEventId && 
                r.CandidateId == candidateId)), Times.Once);
            _mockLogger.VerifyInformationWasCalled("TeachingEventRegistrationJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled("TeachingEventRegistrationJob - Succeeded");
        }

        [Fact]
        public void Run_OnFailure_EmailsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(JobConfiguration.Attempts - 1);

            _job.Run(_attendee, _teachingEventId, null);

            _mockCrm.Verify(mock => mock.Save(It.IsAny<TeachingEventRegistration>()), Times.Never);
            _mockNotifyService.Verify(mock => mock.SendEmailAsync(_attendee.Email,
                NotifyService.TeachingEventRegistrationFailedEmailTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
            _mockLogger.VerifyInformationWasCalled("TeachingEventRegistrationJob - Started (24/24)");
            _mockLogger.VerifyInformationWasCalled("TeachingEventRegistrationJob - Deleted");
        }
    }
}