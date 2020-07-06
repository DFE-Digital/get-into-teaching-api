using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
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
        private readonly TeachingEventRegistrationRequest _request;
        private readonly Guid _teachingEventId;
        private readonly TeachingEventRegistrationJob _job;

        public TeachingEventRegistrationJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _teachingEventId = Guid.NewGuid();
            _mockLogger = new Mock<ILogger<TeachingEventRegistrationJob>>();
            _request = new TeachingEventRegistrationRequest()
            {
                Email = "test@test.com",
                FirstName = "first",
                LastName = "last",
                Telephone = "1234",
                AddressPostcode = "KY11 9YU",
                PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() }
            };
            _job = new TeachingEventRegistrationJob(new Env(), _mockCrm.Object, _mockNotifyService.Object,
                _mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public void Run_OnSuccessWithExistingCandidate_SavesRegistration()
        {
            var candidateId = Guid.NewGuid();
            var candidate = new Candidate() { Id = candidateId };
            _request.CandidateId = candidateId;
            _mockCrm.Setup(m => m.Save(It.Is<Candidate>(c => VerifyUpdatedCandidate(c, _request.Telephone))));
            _mockCrm.Setup(m => m.GetCandidate(candidateId)).Returns(candidate);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_request, _teachingEventId, null);

            _mockCrm.Verify(mock => mock.Save(It.Is<TeachingEventRegistration>(r =>
                r.EventId == _teachingEventId &&
                r.CandidateId == candidate.Id)), Times.Once);
            _mockLogger.VerifyInformationWasCalled("TeachingEventRegistrationJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled("TeachingEventRegistrationJob - Succeeded");
        }

        [Fact]
        public void Run_OnSuccessWithExistingCandidateAndNewTelephoneIsNull_DoesNotOverwriteTelephone()
        {
            var candidateId = Guid.NewGuid();
            var candidate = new Candidate() { Id = candidateId, Telephone = "1234" };
            _request.CandidateId = candidateId;
            _request.Telephone = null;
            _mockCrm.Setup(m => m.Save(It.Is<Candidate>(c => VerifyUpdatedCandidate(c, c.Telephone))));
            _mockCrm.Setup(m => m.GetCandidate(candidateId)).Returns(candidate);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_request, _teachingEventId, null);

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
            _mockCrm.Setup(m => m.Save(It.Is<Candidate>(c => VerifyUpdatedCandidate(c, _request.Telephone))))
                .Callback<BaseModel>(c => c.Id = candidateId);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_request, _teachingEventId, null);

            _mockCrm.Verify(mock => mock.Save(It.Is<TeachingEventRegistration>(r =>
                r.EventId == _teachingEventId &&
                r.CandidateId == candidateId)), Times.Once);
            _mockCrm.Verify(m => m.GetCandidate(It.IsAny<Guid>()), Times.Never);
            _mockLogger.VerifyInformationWasCalled("TeachingEventRegistrationJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled("TeachingEventRegistrationJob - Succeeded");
        }

        [Fact]
        public void Run_OnFailure_EmailsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(23);

            _job.Run(_request, _teachingEventId, null);

            _mockCrm.Verify(mock => mock.Save(It.IsAny<TeachingEventRegistration>()), Times.Never);
            _mockNotifyService.Verify(mock => mock.SendEmailAsync(_request.Email,
                NotifyService.TeachingEventRegistrationFailedEmailTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
            _mockLogger.VerifyInformationWasCalled("TeachingEventRegistrationJob - Started (24/24)");
            _mockLogger.VerifyInformationWasCalled("TeachingEventRegistrationJob - Deleted");
        }

        [Fact]
        public void Run_WithNewSubscriber_CreatesSubscription()
        {
            var candidateId = Guid.NewGuid();
            _mockCrm.Setup(m => m.Save(It.Is<Candidate>(c => c.Subscriptions.First().TypeId == (int)Subscription.ServiceType.Event)))
                .Callback<BaseModel>(c => c.Id = candidateId);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_request, _teachingEventId, null);
        }

        [Fact]
        public void Run_WithExistingSubscriber_DoesNotCreatesSubscription()
        {
            var candidateId = Guid.NewGuid();
            var candidate = new Candidate() { Id = candidateId };
            _request.CandidateId = candidateId;
            _mockCrm.Setup(m => m.GetCandidate(candidateId)).Returns(candidate);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _mockCrm.Setup(m => m.IsCandidateSubscribedToServiceOfType(candidateId,
                (int)Subscription.ServiceType.Event)).Returns(true);

            _job.Run(_request, _teachingEventId, null);

            _mockCrm.Verify(m => m.Save(It.Is<Candidate>(c => c.Subscriptions.Count == 0)));
        }

        private bool VerifyUpdatedCandidate(Candidate candidate, string expectedTelephone)
        {
            return candidate.FirstName == _request.FirstName && 
                   candidate.LastName == _request.LastName && 
                   candidate.Email == _request.Email && 
                   candidate.Telephone == expectedTelephone && 
                   candidate.AddressPostcode == _request.AddressPostcode &&
                   candidate.PrivacyPolicy == _request.PrivacyPolicy;
        }
    }
}