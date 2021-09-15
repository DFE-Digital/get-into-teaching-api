using System;
using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
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
        private readonly Candidate _candidate;
        private readonly CandidateAttributes _attributes;
        private readonly IList<ApplicationForm> _forms;

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
            _forms = new List<ApplicationForm>()
            {
                new ApplicationForm()
                {
                    Id = 1,
                    CreatedAt = new DateTime(2021, 1, 4),
                    UpdatedAt = new DateTime(2021, 1, 5),
                    ApplicationStatus = "never_signed_in",
                    ApplicationPhase = "apply_2",
                },
                new ApplicationForm()
                {
                    Id = 2,
                    CreatedAt = new DateTime(2021, 1, 3),
                    ApplicationStatus = "awaiting_candidate_response",
                    ApplicationPhase = "apply_1",
                },
            };
            _attributes = new CandidateAttributes()
            {
                Email = "email@address.com",
                CreatedAt = new DateTime(2021, 1, 1, 10, 0, 0),
                UpdatedAt = new DateTime(2021, 1, 2, 11, 12, 13),
                ApplicationForms = _forms,
            };
            _candidate = new Candidate()
            { 
                Id = "12345",
                Attributes = _attributes
            };
        }

        [Fact]
        public void Run_OnSuccessWithExistingCandidate_SavesCandidate()
        {
            var match = new GetIntoTeachingApi.Models.Crm.Candidate() { Id = Guid.NewGuid(), Email = _candidate.Attributes.Email };
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
            _mockCrm.Setup(m => m.MatchCandidate(_candidate.Attributes.Email)).Returns(match);
            _mockCrm.Setup(m => m.Save(It.Is<GetIntoTeachingApi.Models.Crm.Candidate>(
                c => c.Id == match.Id
                && c.FindApplyId == _candidate.Id
                && c.Email == _attributes.Email
                && c.FindApplyStatusId == (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.NeverSignedIn
                && c.FindApplyPhaseId == (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply2
                && c.FindApplyCreatedAt == _attributes.CreatedAt
                && c.FindApplyUpdatedAt == _attributes.UpdatedAt)));
            _mockCrm.Setup(m => m.Save(It.IsAny<GetIntoTeachingApi.Models.Crm.ApplicationForm>()));

            _job.Run(_candidate);

            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Started - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Hit - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Succeeded - {_candidate.Id}");
        }

        [Fact]
        public void Run_OnSuccessWhenWithNewCandidate_SavesCandidate()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
            _mockCrm.Setup(m => m.MatchCandidate(_candidate.Attributes.Email)).Returns<Candidate>(null);
            _mockCrm.Setup(m => m.Save(It.Is<GetIntoTeachingApi.Models.Crm.Candidate>(
                c => c.Id == null
                && c.ChannelId == (int)GetIntoTeachingApi.Models.Crm.Candidate.Channel.ApplyForTeacherTraining
                && c.FindApplyId == _candidate.Id
                && c.Email == _attributes.Email
                && c.FindApplyStatusId == (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.NeverSignedIn
                && c.FindApplyPhaseId == (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply2
                && c.FindApplyCreatedAt == _attributes.CreatedAt
                && c.FindApplyUpdatedAt == _attributes.UpdatedAt))).Callback<GetIntoTeachingApi.Models.Crm.BaseModel>(c => c.Id = Guid.NewGuid());
            _mockCrm.Setup(m => m.Save(It.IsAny<GetIntoTeachingApi.Models.Crm.ApplicationForm>()));

            _job.Run(_candidate);

            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Started - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Miss - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Succeeded - {_candidate.Id}");
        }

        [Fact]
        public void Run_OnSuccess_SavesApplicationForms()
        {
            var match = new GetIntoTeachingApi.Models.Crm.Candidate() { Id = Guid.NewGuid(), Email = _candidate.Attributes.Email };
            var existingApplicationForm = new GetIntoTeachingApi.Models.Crm.ApplicationForm() { Id = Guid.NewGuid() };
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
            _mockCrm.Setup(m => m.MatchCandidate(_candidate.Attributes.Email)).Returns(match);
            _mockCrm.Setup(m => m.GetApplicationForm("1")).Returns<GetIntoTeachingApi.Models.Crm.ApplicationForm>(null);
            _mockCrm.Setup(m => m.GetApplicationForm("2")).Returns(existingApplicationForm);
            _mockCrm.Setup(m => m.Save(It.IsAny<GetIntoTeachingApi.Models.Crm.Candidate>()));
            _mockCrm.Setup(m => m.Save(It.Is<GetIntoTeachingApi.Models.Crm.ApplicationForm>(
                f => f.Id == null
                && f.FindApplyId == _forms[0].Id.ToString()
                && f.CreatedAt == _forms[0].CreatedAt
                && f.PhaseId == (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply2
                && f.StatusId == (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.NeverSignedIn
                && f.UpdatedAt == _forms[0].UpdatedAt)));
            _mockCrm.Setup(m => m.Save(It.Is<GetIntoTeachingApi.Models.Crm.ApplicationForm>(
                f => f.Id == existingApplicationForm.Id
                && f.FindApplyId == _forms[1].Id.ToString()
                && f.PhaseId == (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply1
                && f.StatusId == (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.AwaitingCandidateResponse
                && f.CreatedAt == _forms[1].CreatedAt
                && f.UpdatedAt == _forms[1].UpdatedAt)));

            _job.Run(_candidate);

            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Started - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Hit - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Succeeded - {_candidate.Id}");
        }


        [Fact]
        public void Run_OnSuccessWhenThereAreNoApplicationForms_SavesCandidate()
        {
            _candidate.Attributes.ApplicationForms = Array.Empty<ApplicationForm>();

            var match = new GetIntoTeachingApi.Models.Crm.Candidate() { Id = Guid.NewGuid(), Email = _candidate.Attributes.Email };
            var existingApplicationForm = new GetIntoTeachingApi.Models.Crm.ApplicationForm() { Id = Guid.NewGuid() };
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
            _mockCrm.Setup(m => m.MatchCandidate(_candidate.Attributes.Email)).Returns(match);
            _mockCrm.Setup(m => m.Save(It.IsAny<GetIntoTeachingApi.Models.Crm.Candidate>()));

            _job.Run(_candidate);

            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Started - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Hit - {_candidate.Id}");
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