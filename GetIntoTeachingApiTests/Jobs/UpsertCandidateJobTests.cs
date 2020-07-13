using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class UpsertCandidateJobTests
    {
        private readonly Mock<IPerformContextAdapter> _mockContext;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Candidate _candidate;
        private readonly UpsertCandidateJob _job;
        private readonly Mock<ILogger<UpsertCandidateJob>> _mockLogger;

        public UpsertCandidateJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _mockLogger = new Mock<ILogger<UpsertCandidateJob>>();
            _candidate = new Candidate() { Email = "test@test.com" };
            _job = new UpsertCandidateJob(new Env(), _mockCrm.Object, _mockNotifyService.Object,
                _mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public void Run_OnSuccess_SavesCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(_candidate), Times.Once);
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Succeeded");
        }

        [Fact]
        public void Run_WithTeachingEventRegistrationsOnSuccess_SavesTeachingEventRegistrations()
        {
            var candidateId = Guid.NewGuid();
            var registration = new TeachingEventRegistration() { EventId = Guid.NewGuid() };
            _candidate.TeachingEventRegistrations.Add(registration);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _mockCrm.Setup(mock => mock.Save(_candidate)).Callback(() => _candidate.Id = candidateId);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(registration), Times.Once);
            registration.CandidateId.Should().Be(candidateId);
        }

        [Fact]
        public void Run_WithExistingActiveMailingListSubscriptionAndNewTeacherTrainingAdviserSubscriptionOnSuccess_DeactivatesMailingListSubscription()
        {
            var candidateId = Guid.NewGuid();
            var teacherTrainingAdviserSubscription = new Subscription()
            {
                TypeId = (int)Subscription.ServiceType.TeacherTrainingAdviser
            };
            var mailingListSubscription = new Subscription()
            {
                TypeId = (int)Subscription.ServiceType.MailingList
            };
            var existingCandidate = new Candidate() { Subscriptions = new List<Subscription> { mailingListSubscription } };
            _candidate.Id = candidateId;
            _candidate.Subscriptions = new List<Subscription>() { teacherTrainingAdviserSubscription };
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _mockCrm.Setup(mock => mock.Save(_candidate));
            _mockCrm.Setup(mock => mock.GetCandidate(candidateId)).Returns(existingCandidate);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(_candidate), Times.Once);
            _candidate.Subscriptions.Any(s => s.TypeId == (int)Subscription.ServiceType.MailingList && 
                s.StatusId == (int)Subscription.SubscriptionStatus.Inactive).Should().BeTrue();
        }

        [Fact]
        public void Run_OnFailure_EmailsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(23);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(_candidate), Times.Never);
            _mockNotifyService.Verify(mock => mock.SendEmailAsync(_candidate.Email,
                NotifyService.CandidateRegistrationFailedEmailTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Started (24/24)");
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Deleted");
        }
    }
}
