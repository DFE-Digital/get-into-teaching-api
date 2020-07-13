using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Jobs
{
    public class UpsertCandidateJob : BaseJob
    {
        private readonly ICrmService _crm;
        private readonly INotifyService _notifyService;
        private readonly IPerformContextAdapter _contextAdapter;
        private readonly ILogger<UpsertCandidateJob> _logger;

        public UpsertCandidateJob(
            IEnv env,
            ICrmService crm,
            INotifyService notifyService,
            IPerformContextAdapter contextAdapter,
            ILogger<UpsertCandidateJob> logger)
            : base(env)
        {
            _crm = crm;
            _notifyService = notifyService;
            _contextAdapter = contextAdapter;
            _logger = logger;
        }

        public void Run(Candidate candidate, PerformContext context)
        {
            _logger.LogInformation($"UpsertCandidateJob - Started ({AttemptInfo(context, _contextAdapter)})");

            if (IsLastAttempt(context, _contextAdapter))
            {
                var personalisation = new Dictionary<string, dynamic>();

                // We fire and forget the email, ensuring the job succeeds.
                _notifyService.SendEmailAsync(
                    candidate.Email,
                    NotifyService.CandidateRegistrationFailedEmailTemplateId,
                    personalisation);
                _logger.LogInformation("UpsertCandidateJob - Deleted");
            }
            else
            {
                var registrations = ClearTeachingEventRegistrations(candidate);
                ReconcileActiveSubscriptions(candidate);
                SaveCandidate(candidate);
                SaveTeachingEventRegistrations(registrations, candidate);

                _logger.LogInformation("UpsertCandidateJob - Succeeded");
            }
        }

        private void SaveCandidate(Candidate candidate)
        {
            _crm.Save(candidate);
        }

        private void ReconcileActiveSubscriptions(Candidate candidate)
        {
            if (candidate.Id == null)
            {
                return;
            }

            var existingCandidate = _crm.GetCandidate((Guid)candidate.Id);
            DeactivateMailingListOnSubscribingToTeacherTrainingAdviser(candidate, existingCandidate);
        }

        private void DeactivateMailingListOnSubscribingToTeacherTrainingAdviser(Candidate candidate, Candidate existingCandidate)
        {
            var isSubscribingToTeacherTrainingAdviserService =
                candidate.HasActiveSubscriptionToService(Subscription.ServiceType.TeacherTrainingAdviser);

            if (!isSubscribingToTeacherTrainingAdviserService)
            {
                return;
            }

            var hasActiveMailingListServiceSubscription = existingCandidate
                .HasActiveSubscriptionToService(Subscription.ServiceType.MailingList);

            if (!hasActiveMailingListServiceSubscription)
            {
                return;
            }

            var existingMailingListSubscription = existingCandidate.GetActiveSubscriptionToService(Subscription.ServiceType.MailingList);
            existingMailingListSubscription.StatusId = (int)Subscription.SubscriptionStatus.Inactive;
            candidate.Subscriptions.Add(existingMailingListSubscription);
        }

        private IEnumerable<TeachingEventRegistration> ClearTeachingEventRegistrations(Candidate candidate)
        {
            // Due to reasons unknown the candidate registrations relationship can't be deep-inserted
            // in the same way we do for other relationships - we need to explicitly save them against
            // the candidate instead.
            var teachingEventRegistrations = new List<TeachingEventRegistration>(candidate.TeachingEventRegistrations);
            candidate.TeachingEventRegistrations.Clear();
            return teachingEventRegistrations;
        }

        private void SaveTeachingEventRegistrations(IEnumerable<TeachingEventRegistration> registrations, Candidate candidate)
        {
            foreach (var registration in registrations)
            {
                registration.CandidateId = (Guid)candidate.Id;
                _crm.Save(registration);
            }
        }
    }
}
