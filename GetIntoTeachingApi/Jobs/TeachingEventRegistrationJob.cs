using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Hangfire.Server;

namespace GetIntoTeachingApi.Jobs
{
    public class TeachingEventRegistrationJob : BaseJob
    {
        private readonly ICrmService _crm;
        private readonly INotifyService _notifyService;
        private readonly IPerformContextAdapter _contextAdapter;

        public TeachingEventRegistrationJob(ICrmService crm, INotifyService notifyService,
            IPerformContextAdapter contextAdapter)
        {
            _crm = crm;
            _notifyService = notifyService;
            _contextAdapter = contextAdapter;
        }

        public void Run(ExistingCandidateRequest attendee, Guid teachingEventId, PerformContext context)
        {
            if (IsLastAttempt(context, _contextAdapter))
                NotifyAttendeeOfFailure(attendee);
            else
                RegisterAttendeeForEvent(attendee, teachingEventId);
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
            _notifyService.SendEmail(
                attendee.Email,
                NotifyService.TeachingEventRegistrationFailedTemplateId,
                new Dictionary<string, dynamic>());
        }

        private Candidate CreateCandidate(ExistingCandidateRequest attendee)
        {
            var candidate = new Candidate()
            {
                Email = attendee.Email, 
                FirstName = attendee.FirstName, 
                LastName = attendee.LastName,
                DateOfBirth = attendee.DateOfBirth
            };

            _crm.Save(candidate);

            return candidate;
        }
    }
}