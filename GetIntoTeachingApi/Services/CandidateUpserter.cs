using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Utils;
using Hangfire;

namespace GetIntoTeachingApi.Services
{
    public class CandidateUpserter : ICandidateUpserter
    {
        private readonly ICrmService _crm;
        private readonly IBackgroundJobClient _jobClient;

        public CandidateUpserter(ICrmService crm, IBackgroundJobClient jobClient)
        {
            _crm = crm;
            _jobClient = jobClient;
        }

        public void Upsert(Candidate candidate)
        {
            var registrations = ClearTeachingEventRegistrations(candidate);
            var phoneCall = ClearPhoneCall(candidate);
            var privacyPolicy = ClearPrivacyPolicy(candidate);
            var qualifications = ClearQualifications(candidate);
            var pastTeachingPositions = ClearPastTeachingPositions(candidate);
            var applicationForms = ClearApplicationForms(candidate);
            var schoolExperiences = ClearSchoolExperiences(candidate);

            SaveCandidate(candidate);

            SaveQualifications(qualifications, candidate);
            SavePastTeachingPositions(pastTeachingPositions, candidate);
            SaveApplicationForms(applicationForms, candidate);
            SaveTeachingEventRegistrations(registrations, candidate);
            SavePrivacyPolicy(privacyPolicy, candidate);
            SavePhoneCall(phoneCall, candidate);
            SaveSchoolExperiences(schoolExperiences, candidate);

            IncrementCallbackBookingQuotaNumberOfBookings(phoneCall);
        }

        private static IEnumerable<TeachingEventRegistration> ClearTeachingEventRegistrations(Candidate candidate)
        {
            // Due to reasons unknown the event registrations relationship can't be deep-inserted
            // in the same way we do for other relationships - we need to explicitly save them against
            // the candidate instead.
            var teachingEventRegistrations = new List<TeachingEventRegistration>(candidate.TeachingEventRegistrations);
            candidate.TeachingEventRegistrations.Clear();
            return teachingEventRegistrations;
        }

        private static IEnumerable<CandidatePastTeachingPosition> ClearPastTeachingPositions(Candidate candidate)
        {
            var pastTeachingPositions = new List<CandidatePastTeachingPosition>(candidate.PastTeachingPositions);
            candidate.PastTeachingPositions.Clear();
            return pastTeachingPositions;
        }

        private static IEnumerable<ApplicationForm> ClearApplicationForms(Candidate candidate)
        {
            var applicationForms = new List<ApplicationForm>(candidate.ApplicationForms);
            candidate.ApplicationForms.Clear();
            return applicationForms;
        }

        private static IEnumerable<CandidateQualification> ClearQualifications(Candidate candidate)
        {
            var qualifications = new List<CandidateQualification>(candidate.Qualifications);
            candidate.Qualifications.Clear();
            return qualifications;
        }

        private static IEnumerable<CandidateSchoolExperience> ClearSchoolExperiences(Candidate candidate)
        {
            var schoolExperiences = new List<CandidateSchoolExperience>(candidate.SchoolExperiences);
            candidate.SchoolExperiences.Clear();
            return schoolExperiences;
        }

        private static PhoneCall ClearPhoneCall(Candidate candidate)
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

        private static CandidatePrivacyPolicy ClearPrivacyPolicy(Candidate candidate)
        {
            if (candidate.PrivacyPolicy == null)
            {
                return null;
            }

            var privacyPolicy = candidate.PrivacyPolicy;
            candidate.PrivacyPolicy = null;
            return privacyPolicy;
        }

        private void SaveCandidate(Candidate candidate)
        {
            _crm.Save(candidate);
        }

        private void SaveTeachingEventRegistrations(IEnumerable<TeachingEventRegistration> registrations, Candidate candidate)
        {
            foreach (var registration in registrations)
            {
                registration.CandidateId = (Guid)candidate.Id;
                string json = registration.SerializeChangeTracked();
                _jobClient.Enqueue<UpsertModelWithCandidateIdJob<TeachingEventRegistration>>((x) => x.Run(json, null));
            }
        }

        private void SaveQualifications(IEnumerable<CandidateQualification> qualifications, Candidate candidate)
        {
            foreach (var qualification in qualifications)
            {
                qualification.CandidateId = (Guid)candidate.Id;
                string json = qualification.SerializeChangeTracked();
                _jobClient.Enqueue<UpsertModelWithCandidateIdJob<CandidateQualification>>((x) => x.Run(json, null));
            }
        }

        private void SaveApplicationForms(IEnumerable<ApplicationForm> applicationForms, Candidate candidate)
        {
            foreach (var applicationForm in applicationForms)
            {
                applicationForm.CandidateId = (Guid)candidate.Id;
                string json = applicationForm.SerializeChangeTracked();
                _jobClient.Enqueue<UpsertModelWithCandidateIdJob<ApplicationForm>>((x) => x.Run(json, null));
            }
        }

        private void SavePastTeachingPositions(IEnumerable<CandidatePastTeachingPosition> pastTeachingPositions, Candidate candidate)
        {
            foreach (var pastTeachingPosition in pastTeachingPositions)
            {
                pastTeachingPosition.CandidateId = (Guid)candidate.Id;
                string json = pastTeachingPosition.SerializeChangeTracked();
                _jobClient.Enqueue<UpsertModelWithCandidateIdJob<CandidatePastTeachingPosition>>((x) => x.Run(json, null));
            }
        }

        private void SaveSchoolExperiences(IEnumerable<CandidateSchoolExperience> schoolExperiences, Candidate candidate)
        {
            foreach (var schoolExperience in schoolExperiences)
            {
                schoolExperience.CandidateId = (Guid)candidate.Id;
                string json = schoolExperience.SerializeChangeTracked();
                _jobClient.Enqueue<UpsertModelWithCandidateIdJob<CandidateSchoolExperience>>((x) => x.Run(json, null));
            }
        }

        private void IncrementCallbackBookingQuotaNumberOfBookings(PhoneCall phoneCall)
        {
            if (phoneCall != null)
            {
                _jobClient.Enqueue<ClaimCallbackBookingSlotJob>((x) => x.Run(phoneCall.ScheduledAt, null));
            }
        }

        private void SavePhoneCall(PhoneCall phoneCall, Candidate candidate)
        {
            if (phoneCall == null)
            {
                return;
            }

            phoneCall.CandidateId = candidate.Id.ToString();
            string json = phoneCall.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertModelWithCandidateIdJob<PhoneCall>>((x) => x.Run(json, null));
        }

        private void SavePrivacyPolicy(CandidatePrivacyPolicy privacyPolicy, Candidate candidate)
        {
            if (privacyPolicy == null)
            {
                return;
            }

            privacyPolicy.CandidateId = (Guid)candidate.Id;
            string json = privacyPolicy.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>>((x) => x.Run(json, null));
        }
    }
}
