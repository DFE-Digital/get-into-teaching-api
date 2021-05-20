using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services
{
    public class CandidateUpserter : ICandidateUpserter
    {
        private readonly ICrmService _crm;

        public CandidateUpserter(ICrmService crm)
        {
            _crm = crm;
        }

        public void Upsert(Candidate candidate)
        {
            var registrations = ClearTeachingEventRegistrations(candidate);
            var phoneCall = ClearPhoneCall(candidate);
            SaveCandidate(candidate);
            SaveTeachingEventRegistrations(registrations, candidate);
            SavePhoneCall(phoneCall, candidate);
            IncrementCallbackBookingQuotaNumberOfBookings(phoneCall);
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
            if (candidate.PhoneCall == null)
            {
                return null;
            }

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
