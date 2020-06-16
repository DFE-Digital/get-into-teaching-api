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

        public TeachingEventRegistrationJob(IEnv env, ICrmService crm, INotifyService notifyService,
            IPerformContextAdapter contextAdapter, ILogger<TeachingEventRegistrationJob> logger)
            : base(env)
        {
            _crm = crm;
            _logger = logger;
            _notifyService = notifyService;
            _contextAdapter = contextAdapter;
        }

        public void Run(ExistingCandidateRequest attendee, Guid teachingEventId, PerformContext context)
        {
            _logger.LogInformation($"TeachingEventRegistrationJob - Started ({AttemptInfo(context, _contextAdapter)})");

            if (IsLastAttempt(context, _contextAdapter))
            {
                NotifyAttendeeOfFailure(attendee);
                _logger.LogInformation("TeachingEventRegistrationJob - Deleted");
            }
            else
            {
                RegisterAttendeeForEvent(attendee, teachingEventId);
                _logger.LogInformation("TeachingEventRegistrationJob - Succeeded");
            }
        }

        private void RegisterAttendeeForEvent(ExistingCandidateRequest attendee, Guid teachingEventId)
        {
            var candidate = _crm.GetCandidate(attendee) ?? CreateCandidate(attendee);

            var registration = new TeachingEventRegistration()
            {
                CandidateId = (Guid)candidate.Id,
                EventId = teachingEventId,
            };

            _crm.Save(registration);
        }

        private void NotifyAttendeeOfFailure(ExistingCandidateRequest attendee)
        {
            // We fire and forget the email, ensuring the job succeeds.
            _notifyService.SendEmailAsync(
                attendee.Email,
                NotifyService.TeachingEventRegistrationFailedEmailTemplateId,
                new Dictionary<string, dynamic>());
        }

        private Candidate CreateCandidate(ExistingCandidateRequest attendee)
        {
            var candidate = new Candidate()
            {
                Email = attendee.Email,
                FirstName = attendee.FirstName,
                LastName = attendee.LastName,
                DateOfBirth = attendee.DateOfBirth,
            };

            _crm.Save(candidate);

            return candidate;
        }
    }
}