using System;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.FindApply;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class FindApplyCandidateSyncJobTests
    {
        private readonly Mock<GetIntoTeachingApi.Models.IAppSettings> _mockAppSettings;
        private readonly Mock<ILogger<FindApplyCandidateSyncJob>> _mockLogger;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly FindApplyCandidateSyncJob _job;
        private readonly GetIntoTeachingApi.Models.FindApply.Candidate _candidate;

        public FindApplyCandidateSyncJobTests()
        {
            _mockLogger = new Mock<ILogger<FindApplyCandidateSyncJob>>();
            _mockAppSettings = new Mock<GetIntoTeachingApi.Models.IAppSettings>();
            _mockCrm = new Mock<ICrmService>();
            _job = new FindApplyCandidateSyncJob(
                new Env(),
                _mockLogger.Object,
                _mockCrm.Object,
                _mockAppSettings.Object);
            _candidate = new GetIntoTeachingApi.Models.FindApply.Candidate() { Id = "12345", Attributes = new CandidateAttributes() { Email = "email@address.com" } };
        }

        [Fact]
        public void Run_OnSuccess_SavesCandidate()
        {
            var match = new GetIntoTeachingApi.Models.Crm.Candidate() { Id = Guid.NewGuid(), Email = _candidate.Attributes.Email };
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
            _mockCrm.Setup(m => m.MatchCandidate(_candidate.Attributes.Email)).Returns(match);
            _mockCrm.Setup(m => m.Save(It.Is<GetIntoTeachingApi.Models.Crm.Candidate>(c => c.Id == match.Id && c.FindApplyId == _candidate.Id)));

            _job.Run(_candidate);

            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Started - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Hit - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Succeeded - {_candidate.Id}");
        }

        [Fact]
        public void Run_WhenCandidateNotFound_LogsMiss()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
            _mockCrm.Setup(m => m.MatchCandidate(_candidate.Attributes.Email)).Returns<GetIntoTeachingApi.Models.FindApply.Candidate>(null);

            _job.Run(_candidate);

            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Started - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Miss - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Succeeded - {_candidate.Id}");
        }

        [Fact]
        public void Run_WhenCrmIntegrationPaused_Aborts()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(true);

            Action action = () => _job.Run(_candidate);

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("FindApplyCandidateSyncJob - Aborting (CRM integration paused).");
        }
    }
}