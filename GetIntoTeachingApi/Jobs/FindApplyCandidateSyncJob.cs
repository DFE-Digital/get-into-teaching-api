using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GetIntoTeachingApi.Models.FindApply;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Jobs
{
    public class FindApplyCandidateSyncJob : BaseJob
    {
        private readonly ILogger<FindApplyCandidateSyncJob> _logger;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ICrmService _crm;
        private readonly Models.IAppSettings _appSettings;

        public FindApplyCandidateSyncJob(
            IEnv env,
            IRedisService redis,
            ILogger<FindApplyCandidateSyncJob> logger,
            ICrmService crm,
            IBackgroundJobClient jobClient,
            Models.IAppSettings appSettings)
            : base(env, redis)
        {
            _logger = logger;
            _crm = crm;
            _jobClient = jobClient;
            _appSettings = appSettings;
        }

        public void Run(Candidate findApplyCandidate)
        {
            if (_appSettings.IsCrmIntegrationPaused)
            {
                throw new InvalidOperationException("FindApplyCandidateSyncJob - Aborting (CRM integration paused).");
            }

            _logger.LogInformation("FindApplyCandidateSyncJob - Started - {Id}", findApplyCandidate.Id);
            SyncCandidate(findApplyCandidate);
            _logger.LogInformation("FindApplyCandidateSyncJob - Succeeded - {Id}", findApplyCandidate.Id);
        }

        public void SyncCandidate(Candidate findApplyCandidate)
        {
            var applicationForms = findApplyCandidate.Attributes.ApplicationForms;
            var candidate = Candidate(findApplyCandidate, applicationForms);

            candidate.ApplicationForms = ApplicationForms(applicationForms).ToList();

            string json = candidate.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));
        }

        private Models.Crm.Candidate Candidate(Candidate findApplyCandidate, IEnumerable<ApplicationForm> findApplyApplicationForms)
        {
            var match = _crm.MatchCandidate(findApplyCandidate.Attributes.Email);
            var status = match == null ? "Miss" : "Hit";

            _logger.LogInformation("FindApplyCandidateSyncJob - {Status} - {Id}", status, findApplyCandidate.Id);

            // We persist a new Candidate to ensure we only write the find/apply
            // attributes back to the CRM and not existing attributes on the match.
            var candidate = new Models.Crm.Candidate()
            {
                Id = match?.Id,
                Email = findApplyCandidate.Attributes.Email,
                FindApplyId = findApplyCandidate.Id,
                FindApplyCreatedAt = findApplyCandidate.Attributes.CreatedAt,
                FindApplyUpdatedAt = findApplyCandidate.Attributes.UpdatedAt,
            };

            if (match == null)
            {
                candidate.ChannelId = (int)Models.Crm.Candidate.Channel.ApplyForTeacherTraining;
            }

            var latestApplicationForm = findApplyApplicationForms?.OrderByDescending(f => f.UpdatedAt).FirstOrDefault();

            if (latestApplicationForm == null)
            {
                candidate.FindApplyStatusId = (int)Models.Crm.ApplicationForm.Status.NeverSignedIn;
            }
            else
            {
                candidate.FindApplyStatusId = (int)Enum.Parse(typeof(Models.Crm.ApplicationForm.Status), latestApplicationForm.ApplicationStatus.ToPascalCase());
                candidate.FindApplyPhaseId = (int)Enum.Parse(typeof(Models.Crm.ApplicationForm.Phase), latestApplicationForm.ApplicationPhase.ToPascalCase());
            }

            return candidate;
        }

        private IEnumerable<Models.Crm.ApplicationForm> ApplicationForms(IEnumerable<ApplicationForm> findApplyApplicationForms)
        {
            if (findApplyApplicationForms == null)
            {
                return Array.Empty<Models.Crm.ApplicationForm>();
            }

            return findApplyApplicationForms.Select(findApplyForm =>
            {
                var existingForm = _crm.GetFindApplyModels<Models.Crm.ApplicationForm>(new string[] { findApplyForm.Id.ToString(CultureInfo.CurrentCulture) }).FirstOrDefault();

                var yearId = ((int)Models.Crm.ApplicationForm.RecruitmentCycleYear.Year2020) + (findApplyForm.RecruitmentCycleYear - 2020);

                return new Models.Crm.ApplicationForm()
                {
                    Id = existingForm?.Id,
                    FindApplyId = findApplyForm.Id.ToString(CultureInfo.CurrentCulture),
                    CreatedAt = findApplyForm.CreatedAt,
                    UpdatedAt = findApplyForm.UpdatedAt,
                    SubmittedAt = findApplyForm.SubmittedAt,
                    StatusId = (int)Enum.Parse(typeof(Models.Crm.ApplicationForm.Status), findApplyForm.ApplicationStatus.ToPascalCase()),
                    PhaseId = (int)Enum.Parse(typeof(Models.Crm.ApplicationForm.Phase), findApplyForm.ApplicationPhase.ToPascalCase()),
                    RecruitmentCycleYearId = yearId,
                };
            });
        }
    }
}
