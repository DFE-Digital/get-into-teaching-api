﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
            _job = new CandidateRegistrationJob(new Env(), _mockCrm.Object, _mockNotifyService.Object,
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
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(23);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(_candidate), Times.Never);
            _mockNotifyService.Verify(mock => mock.SendEmailAsync(_candidate.Email,
                NotifyService.CandidateRegistrationFailedEmailTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
            _mockLogger.VerifyInformationWasCalled("CandidateRegistrationJob - Started (24/24)");
            _mockLogger.VerifyInformationWasCalled("CandidateRegistrationJob - Deleted");
        }

        [Fact]
        public void Run_WithNewSubscriber_CreatesServiceSubscription()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_candidate, null);

            var subscription = _candidate.ServiceSubscriptions.FirstOrDefault();

            subscription.Should().NotBeNull();
            subscription.TypeId.Should().Be((int)ServiceSubscription.ServiceType.TeacherTrainingAdviser);
        }

        [Fact]
        public void Run_WithExistingSubscriber_DoesNotCreatesServiceSubscription()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _candidate.Id = Guid.NewGuid();
            _mockCrm.Setup(m => m.IsCandidateSubscribedToServiceOfType((Guid)_candidate.Id, 
                (int)ServiceSubscription.ServiceType.TeacherTrainingAdviser)).Returns(true);

            _job.Run(_candidate, null);

            var subscription = _candidate.ServiceSubscriptions.FirstOrDefault();

            subscription.Should().BeNull();
        }
    }
}
