using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace GetIntoTeachingApi.Jobs
{
    public class UpsertCandidateJob : BaseJob
    {
        private readonly ICrmService _crm;
        private readonly INotifyService _notifyService;
        private readonly IPerformContextAdapter _contextAdapter;
        private readonly IMetricService _metrics;
        private readonly ILogger<UpsertCandidateJob> _logger;

        public UpsertCandidateJob(
            IEnv env,
            ICrmService crm,
            INotifyService notifyService,
            IPerformContextAdapter contextAdapter,
            IMetricService metrics,
            ILogger<UpsertCandidateJob> logger)
            : base(env)
        {
            _crm = crm;
            _notifyService = notifyService;
            _contextAdapter = contextAdapter;
            _metrics = metrics;
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
                var phoneCall = ClearPhoneCall(candidate);
                SaveCandidate(candidate);
                SaveTeachingEventRegistrations(registrations, candidate);
                SavePhoneCall(phoneCall, candidate);
                IncrementCallbackBookingQuotaNumberOfBookings(phoneCall);

                _logger.LogInformation($"UpsertCandidateJob - Succeeded - {candidate.Id}");
            }

            var duration = (DateTime.UtcNow - _contextAdapter.GetJobCreatedAt(context)).TotalSeconds;
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertCandidateJob" }).Observe(duration);
        }

        private void SaveCandidate(Candidate candidate)
        {
            _crm.Save(candidate);
        }

        private IEnumerable<TeachingEventRegistration> ClearTeachingEventRegistrations(Candidate candidate)
        {
            // Due to reasons unknown the event registrations relationship can't be deep-inserted
            // in the same way we do for other relationships - we need to explicitly save them against
            // the candidate instead.
            var teachingEventRegistrations = new List<TeachingEventRegistration>(candidate.TeachingEventRegistrations);
            candidate.TeachingEventRegistrations.Clear();
            return teachingEventRegistrations;
        }

        private PhoneCall ClearPhoneCall(Candidate candidate)
        {
            // Due to reasons unknown the phone call relationship can't be deep-inserted
            // in the same way we do for other relationships - we need to explicitly save them against
            // the candidate instead.
            var phoneCall = candidate.PhoneCall;
            candidate.PhoneCall = null;
            return phoneCall;
        }

        private void SaveTeachingEventRegistrations(IEnumerable<TeachingEventRegistration> registrations, Candidate candidate)
        {
            foreach (var registration in registrations)
            {
                registration.CandidateId = (Guid)candidate.Id;
                _crm.Save(registration);
            }
        }

        private void IncrementCallbackBookingQuotaNumberOfBookings(PhoneCall phoneCall)
        {
            if (phoneCall == null)
            {
                return;
            }

            var quota = _crm.GetCallbackBookingQuota(phoneCall.ScheduledAt);

            if (quota == null || !quota.IsAvailable)
            {
                return;
            }

            quota.NumberOfBookings += 1;

            _crm.Save(quota);
        }

        private void SavePhoneCall(PhoneCall phoneCall, Candidate candidate)
        {
            if (phoneCall == null)
            {
                return;
            }

            phoneCall.CandidateId = candidate.Id.ToString();
            _crm.Save(phoneCall);
        }
    }
}
