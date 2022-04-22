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
    public class FindApplyCandidateSyncV12JobTests
    {
        private readonly Mock<GetIntoTeachingApi.Models.IAppSettings> _mockAppSettings;
        private readonly Mock<ILogger<FindApplyCandidateSyncJob>> _mockLogger;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly FindApplyCandidateSyncV12Job _job;
        private readonly Candidate _candidate;
        private readonly CandidateAttributes _attributes;
        private readonly IList<ApplicationForm> _forms;

        public FindApplyCandidateSyncV12JobTests()
        {
            _mockLogger = new Mock<ILogger<FindApplyCandidateSyncJob>>();
            _mockAppSettings = new Mock<GetIntoTeachingApi.Models.IAppSettings>();
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _job = new FindApplyCandidateSyncV12Job(
                new Env(),
                new Mock<IRedisService>().Object,
                _mockLogger.Object,
                _mockCrm.Object,
                _mockJobClient.Object,
                _mockAppSettings.Object);
            _forms = new List<ApplicationForm>()
            {
                new ApplicationForm()
                {
                    Id = 2,
                    CreatedAt = new DateTime(2021, 1, 3),
                    UpdatedAt = new DateTime(2021, 1, 3),
                    ApplicationStatus = "awaiting_candidate_response",
                    ApplicationPhase = "apply_1",
                    RecruitmentCycleYear = 2021,
                    ApplicationChoices = new ApplicationResponse<IEnumerable<ApplicationChoice>>() { Completed = false },
                    References = new ApplicationResponse<IEnumerable<Reference>>() { Completed = false },
                    Qualifications = new ApplicationResponse<IEnumerable<object>>() { Completed = true },
                    PersonalStatement = new ApplicationResponse<IEnumerable<object>>() { Completed = null },
                },
                new ApplicationForm()
                {
                    Id = 1,
                    CreatedAt = new DateTime(2021, 1, 4),
                    UpdatedAt = new DateTime(2021, 1, 5),
                    SubmittedAt = new DateTime(2021, 1, 6),
                    ApplicationStatus = "never_signed_in",
                    ApplicationPhase = "apply_2",
                    RecruitmentCycleYear = 2022,
                    ApplicationChoices = new ApplicationResponse<IEnumerable<ApplicationChoice>>() { Completed = false },
                    References = new ApplicationResponse<IEnumerable<Reference>>() { Completed = false },
                    Qualifications = new ApplicationResponse<IEnumerable<object>>() { Completed = true },
                    PersonalStatement = new ApplicationResponse<IEnumerable<object>>() { Completed = null },
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
        public void Run_OnSuccessWithExistingCandidate_SetsIdAndQueuesUpsertJobForCandidateWithApplicationForms()
        {
            var match = new GetIntoTeachingApi.Models.Crm.Candidate() { Id = Guid.NewGuid(), Email = _candidate.Attributes.Email };
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
            _mockCrm.Setup(m => m.MatchCandidate(_candidate.Attributes.Email)).Returns(match);

            _job.Run(_candidate);

            var form1 = new GetIntoTeachingApi.Models.Crm.ApplicationForm()
            {
                FindApplyId = _forms[1].Id.ToString(),
                CreatedAt = _forms[1].CreatedAt,
                PhaseId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply2,
                StatusId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.NeverSignedIn,
                RecruitmentCycleYearId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.RecruitmentCycleYear.Year2022,
                UpdatedAt = _forms[1].UpdatedAt,
                SubmittedAt = _forms[1].SubmittedAt,
                ApplicationChoicesCompleted = _forms[1].ApplicationChoices.Completed,
                ReferencesCompleted = _forms[1].References.Completed,
                PersonalStatementCompleted = _forms[1].PersonalStatement.Completed,
                QualificationsCompleted = _forms[1].Qualifications.Completed,
            };

            var form2 = new GetIntoTeachingApi.Models.Crm.ApplicationForm()
            {
                FindApplyId = _forms[0].Id.ToString(),
                PhaseId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply1,
                StatusId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.AwaitingCandidateResponse,
                RecruitmentCycleYearId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.RecruitmentCycleYear.Year2021,
                CreatedAt = _forms[0].CreatedAt,
                UpdatedAt = _forms[0].UpdatedAt,
                SubmittedAt = null,
                ApplicationChoicesCompleted = _forms[0].ApplicationChoices.Completed,
                ReferencesCompleted = _forms[0].References.Completed,
                PersonalStatementCompleted = _forms[0].PersonalStatement.Completed,
                QualificationsCompleted = _forms[0].Qualifications.Completed,
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
                ApplicationForms = new List<GetIntoTeachingApi.Models.Crm.ApplicationForm>() { form2, form1 },
            };

            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                IsMatch(candidate, (string)job.Args[0])),
                It.IsAny<EnqueuedState>()));

            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncV12Job - Started - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncV12Job - Hit - {_candidate.Id}");
            _mockLogger.VerifyInformationWasCalled($"FindApplyCandidateSyncV12Job - Succeeded - {_candidate.Id}");
        }

        [Fact]
        public void Run_OnSuccessWithNewCandidate_SetsChannelAndQueuesUpsertJobForCandidateWithApplicationForms()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
            _mockCrm.Setup(m => m.MatchCandidate(_candidate.Attributes.Email)).Returns<Candidate>(null);

            _job.Run(_candidate);

            var form1 = new GetIntoTeachingApi.Models.Crm.ApplicationForm()
            {
                FindApplyId = _forms[1].Id.ToString(),
                CreatedAt = _forms[1].CreatedAt,
                PhaseId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply2,
                StatusId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.NeverSignedIn,
                RecruitmentCycleYearId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.RecruitmentCycleYear.Year2022,
                UpdatedAt = _forms[1].UpdatedAt,
                SubmittedAt = _forms[1].SubmittedAt,
                ApplicationChoicesCompleted = _forms[1].ApplicationChoices.Completed,
                ReferencesCompleted = _forms[1].References.Completed,
                PersonalStatementCompleted = _forms[1].PersonalStatement.Completed,
                QualificationsCompleted = _forms[1].Qualifications.Completed,
            };

            var form2 = new GetIntoTeachingApi.Models.Crm.ApplicationForm()
            {
                FindApplyId = _forms[0].Id.ToString(),
                PhaseId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply1,
                StatusId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.AwaitingCandidateResponse,
                RecruitmentCycleYearId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.RecruitmentCycleYear.Year2021,
                CreatedAt = _forms[0].CreatedAt,
                UpdatedAt = _forms[0].UpdatedAt,
                SubmittedAt = null,
                ApplicationChoicesCompleted = _forms[0].ApplicationChoices.Completed,
                ReferencesCompleted = _forms[0].References.Completed,
                PersonalStatementCompleted = _forms[0].PersonalStatement.Completed,
                QualificationsCompleted = _forms[0].Qualifications.Completed,
            };

            var candidate = new GetIntoTeachingApi.Models.Crm.Candidate()
            {
                FindApplyId = _candidate.Id,
                Email = _attributes.Email,
                FindApplyStatusId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.NeverSignedIn,
                FindApplyPhaseId = (int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply2,
                FindApplyCreatedAt = _attributes.CreatedAt,
                FindApplyUpdatedAt = _attributes.UpdatedAt,
                ApplicationForms = new List<GetIntoTeachingApi.Models.Crm.ApplicationForm>() { form2, form1 },
                ChannelId = (int)GetIntoTeachingApi.Models.Crm.Candidate.Channel.ApplyForTeacherTraining,
            };

            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                IsMatch(candidate, (string)job.Args[0])),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Run_WhenCrmIntegrationPaused_Aborts()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(true);

            Action action = () => _job.Run(_candidate);

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("FindApplyCandidateSyncV12Job - Aborting (CRM integration paused).");
        }

        private static bool IsMatch(GetIntoTeachingApi.Models.Crm.Candidate candidateA, string candidateBJson)
        {
            var candidateB = candidateBJson.DeserializeChangeTracked<GetIntoTeachingApi.Models.Crm.Candidate>();
            candidateA.Should().BeEquivalentTo(candidateB);
            return true;
        }
    }
}