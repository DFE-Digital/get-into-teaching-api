using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Models.FindApply;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Jobs
{
    public class FindApplyCandidateSyncJob : BaseJob
    {
        private readonly ILogger<FindApplyCandidateSyncJob> _logger;
        private readonly ICrmService _crm;
        private readonly Models.IAppSettings _appSettings;

        public FindApplyCandidateSyncJob(
            IEnv env,
            ILogger<FindApplyCandidateSyncJob> logger,
            ICrmService crm,
            Models.IAppSettings appSettings)
            : base(env)
        {
            _logger = logger;
            _crm = crm;
            _appSettings = appSettings;
        }

        public void Run(Candidate findApplyCandidate)
        {
            if (_appSettings.IsCrmIntegrationPaused)
            {
                throw new InvalidOperationException("FindApplyCandidateSyncJob - Aborting (CRM integration paused).");
            }

            _logger.LogInformation($"FindApplyCandidateSyncJob - Started - {findApplyCandidate.Id}");
            SyncCandidate(findApplyCandidate);
            _logger.LogInformation($"FindApplyCandidateSyncJob - Succeeded - {findApplyCandidate.Id}");
        }

        public void SyncCandidate(Candidate findApplyCandidate)
        {
            var match = _crm.MatchCandidate(findApplyCandidate.Attributes.Email);

            if (match != null)
            {
                _logger.LogInformation($"FindApplyCandidateSyncJob - Hit - {findApplyCandidate.Id}");

                var candidateId = (Guid)match.Id;
                var applicationForms = findApplyCandidate.Attributes.ApplicationForms;
                UpdateCandidate(candidateId, findApplyCandidate, applicationForms);
                UpsertApplicationForms(candidateId, applicationForms);
            }
            else
            {
                _logger.LogInformation($"FindApplyCandidateSyncJob - Miss - {findApplyCandidate.Id}");
            }
        }

        private void UpdateCandidate(Guid candidateId, Candidate findApplyCandidate, IEnumerable<ApplicationForm> findApplyApplicationForms)
        {
            // We persist a new Candidate to ensure we only write the find/apply
            // attributes back to the CRM and not existing attributes on the match.
            var candidate = new Models.Crm.Candidate()
            {
                Id = candidateId,
                FindApplyId = findApplyCandidate.Id,
                FindApplyCreatedAt = findApplyCandidate.Attributes.CreatedAt,
                FindApplyUpdatedAt = findApplyCandidate.Attributes.UpdatedAt,
            };

            var latestApplicationForm = findApplyApplicationForms?.FirstOrDefault();

            if (latestApplicationForm != null)
            {
                candidate.FindApplyStatusId = (int)Enum.Parse(typeof(Models.Crm.ApplicationForm.Status), latestApplicationForm.ApplicationStatus.ToPascalCase());
                candidate.FindApplyPhaseId = (int)Enum.Parse(typeof(Models.Crm.ApplicationForm.Phase), latestApplicationForm.ApplicationPhase.ToPascalCase());
            }

            _crm.Save(candidate);
        }

        private void UpsertApplicationForms(Guid candidateId, IEnumerable<ApplicationForm> findApplyApplicationForms)
        {
            if (findApplyApplicationForms == null)
            {
                return;
            }

            foreach (var findApplyForm in findApplyApplicationForms)
            {
                var existingForm = _crm.GetApplicationForm(findApplyForm.Id.ToString());

                var form = new Models.Crm.ApplicationForm()
                {
                    Id = existingForm?.Id,
                    CandidateId = candidateId,
                    FindApplyId = findApplyForm.Id.ToString(),
                    CreatedAt = findApplyForm.CreatedAt,
                    UpdatedAt = findApplyForm.UpdatedAt,
                    StatusId = (int)Enum.Parse(typeof(Models.Crm.ApplicationForm.Status), findApplyForm.ApplicationStatus.ToPascalCase()),
                    PhaseId = (int)Enum.Parse(typeof(Models.Crm.ApplicationForm.Phase), findApplyForm.ApplicationPhase.ToPascalCase()),
                };

                _crm.Save(form);
            }
        }
    }
}
