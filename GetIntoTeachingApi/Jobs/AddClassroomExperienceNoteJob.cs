using System;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.SchoolsExperience;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Hangfire.Server;

namespace GetIntoTeachingApi.Jobs
{
    public class AddClassroomExperienceNoteJob : BaseJob
    {
        private const int NumberOfRetriesAllowingForJobQueue = 3;

        private readonly IPerformContextAdapter _contextAdapter;
        private readonly IAppSettings _appSettings;
        private readonly ICrmService _crm;
        private readonly IBackgroundJobClient _jobClient;

        public AddClassroomExperienceNoteJob(
            IEnv env,
            IPerformContextAdapter contextAdapter,
            IAppSettings appSettings,
            ICrmService crm,
            IBackgroundJobClient jobClient)
            : base(env)
        {
            _contextAdapter = contextAdapter;
            _appSettings = appSettings;
            _crm = crm;
            _jobClient = jobClient;
        }

        public void Run(
            PerformContext context,
            ClassroomExperienceNote note,
            Guid candidateId)
        {
            if (_appSettings.IsCrmIntegrationPaused)
            {
                throw new InvalidOperationException($"{GetType().Name} - Aborting (CRM integration paused).");
            }

            var existingCandidate = _crm.GetCandidate(candidateId);

            if (existingCandidate == null)
            {
                throw new InvalidOperationException(BuildExceptionMessage(CurrentAttempt(context, _contextAdapter)));
            }
            else
            {
                // Create a new candidate to encapsulate the actual changes - avoids writing
                // all the existingCandidate fields back to the CRM.
                var candidate = new Candidate()
                {
                    Id = candidateId,
                    ClassroomExperienceNotesRaw = existingCandidate.ClassroomExperienceNotesRaw,
                };

                candidate.AddClassroomExperienceNote(note);

                string json = candidate.SerializeChangeTracked();
                _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));
            }
        }

        private string BuildExceptionMessage(int numberOfRetries)
        {
            var baseMessage = $"{GetType().Name} - Candidate not found";

            return numberOfRetries <= NumberOfRetriesAllowingForJobQueue
                ? baseMessage += " (may be in concurrent job queue)"
                : baseMessage;
        }
    }
}
