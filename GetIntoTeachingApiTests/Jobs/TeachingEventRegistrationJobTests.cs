using System;
using System.Collections.Generic;
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
        private readonly ExistingCandidateRequest _attendee;
        private readonly Guid _teachingEventId;
        private readonly TeachingEventRegistrationJob _job;

        public TeachingEventRegistrationJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _teachingEventId = Guid.NewGuid();
            _attendee = new ExistingCandidateRequest() { Email = "test@test.com", FirstName = "first", LastName = "last" };
            _job = new TeachingEventRegistrationJob(_mockCrm.Object, _mockNotifyService.Object, _mockContext.Object);
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
        }

        [Fact]
        public void Run_OnFailure_EmailsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(JobConfiguration.Attempts);

            _job.Run(_attendee, _teachingEventId, null);

            _mockCrm.Verify(mock => mock.Save(It.IsAny<TeachingEventRegistration>()), Times.Never);
            _mockNotifyService.Verify(mock => mock.SendEmail(_attendee.Email,
                NotifyService.TeachingEventRegistrationFailedTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
        }
    }
}