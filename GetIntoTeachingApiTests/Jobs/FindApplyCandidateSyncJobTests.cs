using System;
using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models.FindApply;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
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
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly FindApplyCandidateSyncJob _job;
        private readonly Candidate _candidate;
        private readonly CandidateAttributes _attributes;
        private readonly IList<ApplicationForm> _forms;

        public FindApplyCandidateSyncJobTests()
        {
            _mockLogger = new Mock<ILogger<FindApplyCandidateSyncJob>>();
            _mockAppSettings = new Mock<GetIntoTeachingApi.Models.IAppSettings>();
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _job = new FindApplyCandidateSyncJob(
                new Env(),
                _mockLogger.Object,
                _mockCrm.Object,
                _mockJobClient.Object,
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
        public void Run_OnSuccessWithExistingCandidate_QueuesUpsertJobForCandidateWithApplicationForms()
        {
            var match = new GetIntoTeachingApi.Models.Crm.Candidate() { Id = Guid.NewGuid(), Email = _candidate.Attributes.Email };
            var existingApplicationForm = new GetIntoTeachingApi.Models.Crm.ApplicationForm() { Id = Guid.NewGuid() };
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
            _mockCrm.Setup(m => m.MatchCandidate(_candidate.Attributes.Email)).Returns(match);
            _mockCrm.Setup(m => m.GetApplicationForm("1")).Returns<GetIntoTeachingApi.Models.Crm.ApplicationForm>(null);
            _mockCrm.Setup(m => m.GetApplicationForm("2")).Returns(existingApplicationForm);

            _job.Run(_candidate);

            var form1 = new GetIntoTeachingApi.Models.Crm.ApplicationForm()
            {
                Id = null,
                FindApplyId = _forms[0].Id.ToString(),
                CreatedAt = _forms[0].CreatedAt,
                PhaseId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply2,
                StatusId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.NeverSignedIn,
                UpdatedAt = _forms[0].UpdatedAt,
            };

            var form2 = new GetIntoTeachingApi.Models.Crm.ApplicationForm()
            {
                Id = existingApplicationForm.Id,
                FindApplyId = _forms[1].Id.ToString(),
                PhaseId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply1,
                StatusId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.AwaitingCandidateResponse,
                CreatedAt = _forms[1].CreatedAt,
                UpdatedAt = _forms[1].UpdatedAt,
            };

            var candidate = new GetIntoTeachingApi.Models.Crm.Candidate()
            {
                Id = match.Id,
                FindApplyId = _candidate.Id,
                Email = _attributes.Email,
                FindApplyStatusId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.NeverSignedIn,
                FindApplyPhaseId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply2,
                FindApplyCreatedAt = _attributes.CreatedAt,
                FindApplyUpdatedAt = _attributes.UpdatedAt,
                ApplicationForms = new List<GetIntoTeachingApi.Models.Crm.ApplicationForm>() { form1, form2 },
            };

            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                IsMatch(candidate, (string)job.Args[0])),
                It.IsAny<EnqueuedState>()));

            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Started - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Hit - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncJob - Succeeded - {_candidate.Id}");
        }

        [Fact]
        public void Run_OnSuccessWithNewCandidateAndNoApplicationForms_QueuesUpsertJobForCandidateWithNeverSignedInStatus()
        {
            _candidate.Attributes.ApplicationForms = Array.Empty<ApplicationForm>();

            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
            _mockCrm.Setup(m => m.MatchCandidate(_candidate.Attributes.Email)).Returns<Candidate>(null);

            _job.Run(_candidate);

            var candidate = new GetIntoTeachingApi.Models.Crm.Candidate()
            {
                Id = null,
                ChannelId = (int)GetIntoTeachingApi.Models.Crm.Candidate.Channel.ApplyForTeacherTraining,
                FindApplyId = _candidate.Id,
                Email = _attributes.Email,
                FindApplyStatusId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.NeverSignedIn,
                FindApplyCreatedAt = _attributes.CreatedAt,
                FindApplyUpdatedAt = _attributes.UpdatedAt,
            };

            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                IsMatch(candidate, (string)job.Args[0])),
                It.IsAny<EnqueuedState>()));

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

        private static bool IsMatch(GetIntoTeachingApi.Models.Crm.Candidate candidateA, string candidateBJson)
        {
            var candidateB = candidateBJson.DeserializeChangeTracked<GetIntoTeachingApi.Models.Crm.Candidate>();
            candidateA.Should().BeEquivalentTo(candidateB);
            return true;
        }
    }
}