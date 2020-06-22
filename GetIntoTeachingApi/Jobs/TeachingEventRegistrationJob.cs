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
    public class TeachingEventRegistrationJob : BaseJob
    {
        private readonly ICrmService _crm;
        private readonly INotifyService _notifyService;
        private readonly IPerformContextAdapter _contextAdapter;
        private readonly ILogger<TeachingEventRegistrationJob> _logger;

        public TeachingEventRegistrationJob(
            IEnv env,
            ICrmService crm,
            INotifyService notifyService,
            IPerformContextAdapter contextAdapter,
            ILogger<TeachingEventRegistrationJob> logger)
            : base(env)
        {
            _crm = crm;
            _logger = logger;
            _notifyService = notifyService;
            _contextAdapter = contextAdapter;
        }

        public void Run(TeachingEventRegistrationRequest request, Guid teachingEventId, PerformContext context)
        {
            _logger.LogInformation($"TeachingEventRegistrationJob - Started ({AttemptInfo(context, _contextAdapter)})");

            if (IsLastAttempt(context, _contextAdapter))
            {
                NotifyAttendeeOfFailure(request);
                _logger.LogInformation("TeachingEventRegistrationJob - Deleted");
            }
            else
            {
                RegisterAttendeeForEvent(request, teachingEventId);
                _logger.LogInformation("TeachingEventRegistrationJob - Succeeded");
            }
        }

        private void RegisterAttendeeForEvent(TeachingEventRegistrationRequest request, Guid teachingEventId)
        {
            var candidate = FindOrCreateCandidate(request.CandidateId);

            var registration = new TeachingEventRegistration()
            {
                EventId = teachingEventId,
            };

            candidate.Email = request.Email;
            candidate.FirstName = request.FirstName;
            candidate.LastName = request.LastName;
            candidate.Telephone = request.Telephone ?? candidate.Telephone;
            candidate.AddressPostcode = request.AddressPostcode;
            candidate.PrivacyPolicy = request.PrivacyPolicy;
            candidate.RegisteredTeachingEvents = new List<TeachingEventRegistration> { registration };

            _crm.Save(candidate);
        }

        private void NotifyAttendeeOfFailure(TeachingEventRegistrationRequest request)
        {
            // We fire and forget the email, ensuring the job succeeds.
            _notifyService.SendEmailAsync(
                request.Email,
                NotifyService.TeachingEventRegistrationFailedEmailTemplateId,
                new Dictionary<string, dynamic>());
        }

        private Candidate FindOrCreateCandidate(Guid? candidateId)
        {
            return candidateId != null ? _crm.GetCandidate((Guid)candidateId) : new Candidate();
        }
    }
}