using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Jobs.CandidateSanitisation;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Utils;
using Hangfire;
using static GetIntoTeachingApi.Models.Crm.Candidate;

namespace GetIntoTeachingApi.Services
{
    public class CandidateUpserter : ICandidateUpserter
    {
        private readonly ICrmService _crm;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ICrmModelSanitisationRulesHandler _sanitisationRulesHandler;

        public CandidateUpserter(ICrmService crm, IBackgroundJobClient jobClient,
            ICrmModelSanitisationRulesHandler sanitisationRulesHandler)
        {
            _crm = crm;
            _jobClient = jobClient;
            _sanitisationRulesHandler = sanitisationRulesHandler;
        }

        public void Upsert(Candidate candidate)
        {
            candidate = _sanitisationRulesHandler.SanitiseCandidateWithRules(candidate);
            
            // TODO: this code should be refactored in line with Spencer's recommendations
            var registrations = ClearTeachingEventRegistrations(candidate);
            var phoneCall = ClearPhoneCall(candidate);
            var privacyPolicy = ClearPrivacyPolicy(candidate);
            var qualifications = ClearQualifications(candidate);
            var pastTeachingPositions = ClearPastTeachingPositions(candidate);
            var applicationForms = ClearApplicationForms(candidate);
            var schoolExperiences = ClearSchoolExperiences(candidate);
            var contactChannelCreations = ClearContactChannelCreations(candidate);

            PreventCandidateEmailFromBeingOverwritten(candidate);
            UpdateEventSubscriptionType(candidate);

            SaveCandidate(candidate);
            SaveQualifications(qualifications, candidate);
            
            SavePastTeachingPositions(pastTeachingPositions, candidate);
            SaveApplicationForms(applicationForms, candidate);
            SaveTeachingEventRegistrations(registrations, candidate);
            SavePrivacyPolicy(privacyPolicy, candidate);
            SavePhoneCall(phoneCall, candidate);
            SaveSchoolExperiences(schoolExperiences, candidate);
            SaveContactChannelCreation(contactChannelCreations, candidate);

            IncrementCallbackBookingQuotaNumberOfBookings(phoneCall);
            
            // Re-add qualifications back to candidate object to ensure it is correctly returned
            AddQualifications(qualifications, candidate);
        }

        private static List<TeachingEventRegistration> ClearTeachingEventRegistrations(Candidate candidate)
        {
            // Due to reasons unknown the event registrations relationship can't be deep-inserted
            // in the same way we do for other relationships - we need to explicitly save them against
            // the candidate instead.
            var teachingEventRegistrations = new List<TeachingEventRegistration>(candidate.TeachingEventRegistrations);
            candidate.TeachingEventRegistrations.Clear();
            return teachingEventRegistrations;
        }

        private static List<CandidatePastTeachingPosition> ClearPastTeachingPositions(Candidate candidate)
        {
            var pastTeachingPositions = new List<CandidatePastTeachingPosition>(candidate.PastTeachingPositions);
            candidate.PastTeachingPositions.Clear();
            return pastTeachingPositions;
        }

        private static List<ApplicationForm> ClearApplicationForms(Candidate candidate)
        {
            var applicationForms = new List<ApplicationForm>(candidate.ApplicationForms);
            candidate.ApplicationForms.Clear();
            return applicationForms;
        }

        private static List<CandidateQualification> ClearQualifications(Candidate candidate)
        {
            var qualifications = new List<CandidateQualification>(candidate.Qualifications);
            candidate.Qualifications.Clear();
            return qualifications;
        }
        
        private static void AddQualifications(IEnumerable<CandidateQualification> candidateQualifications, Candidate candidate)
        {
            candidate.Qualifications.AddRange(candidateQualifications);
        }

        private static List<CandidateSchoolExperience> ClearSchoolExperiences(Candidate candidate)
        {
            var schoolExperiences = new List<CandidateSchoolExperience>(candidate.SchoolExperiences);
            candidate.SchoolExperiences.Clear();
            return schoolExperiences;
        }
        
        private static List<ContactChannelCreation> ClearContactChannelCreations(Candidate candidate)
        {
            List<ContactChannelCreation> contactChannelCreations =
                new(candidate.ContactChannelCreations);
            candidate.ContactChannelCreations.Clear();

            return contactChannelCreations;
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

        private void PreventCandidateEmailFromBeingOverwritten(Candidate candidate)
        {
            if (candidate.Id == null)
            {
                return;
            }

            var existingCandidate = _crm.GetCandidate((Guid)candidate.Id);

            if (existingCandidate != null)
            {
                candidate.Email = existingCandidate.Email;
            }
        }

        private void UpdateEventSubscriptionType(Candidate candidate)
        {
            var changingEventSubscriptionType = candidate.Id != null && candidate.EventsSubscriptionTypeId != null;

            if (changingEventSubscriptionType && _crm.CandidateAlreadyHasLocalEventSubscriptionType((Guid)candidate.Id))
            {
                // Never down-grade to a 'single event' subscription type from
                // a 'local event' subscription type.
                candidate.EventsSubscriptionTypeId = (int)SubscriptionType.LocalEvent;
            }
        }

        private void SaveCandidate(Candidate candidate)
        {
            candidate.IsNewRegistrant = candidate.Id == null;
            _crm.Save(candidate);
        }
        
        private void SaveQualifications(IEnumerable<CandidateQualification> qualifications, Candidate candidate)
        {
            foreach (var qualification in qualifications)
            {
                // only add the degree qualification to the CRM if it doesn't already exist
                if (!_crm.CandidateHasDegreeQualification((Guid)candidate.Id, CandidateQualification.DegreeType.Degree,
                        qualification.DegreeSubject))
                {
                    qualification.CandidateId = (Guid)candidate.Id;
                    // call the CRM immediate so we get qualification ID and prevent duplicate records from being created
                    _crm.Save(qualification);
                }
            }
        }

        private void SaveTeachingEventRegistrations(IEnumerable<TeachingEventRegistration> registrations, Candidate candidate)
        {
            foreach (var registration in registrations)
            {
                registration.CandidateId = (Guid)candidate.Id;

                // Skip over if the candidate has already registered for this event.
                if (!_crm.CandidateYetToRegisterForTeachingEvent(registration.CandidateId, registration.EventId))
                {
                    continue;
                }

                string json = registration.SerializeChangeTracked();
                _jobClient.Enqueue<UpsertModelWithCandidateIdJob<TeachingEventRegistration>>((x) => x.Run(json, null));
            }
        }
        

        private void SaveApplicationForms(IEnumerable<ApplicationForm> applicationForms, Candidate candidate)
        {
            foreach (var applicationForm in applicationForms)
            {
                applicationForm.CandidateId = (Guid)candidate.Id;
                string json = applicationForm.SerializeChangeTracked();
                _jobClient.Enqueue<UpsertApplicationFormJob>((x) => x.Run(json, null));
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

            // Ignore if they have already accepted this privacy policy.
            if (candidate.Id != null && !_crm.CandidateYetToAcceptPrivacyPolicy((Guid)candidate.Id, privacyPolicy.AcceptedPolicyId))
            {
                return;
            }

            privacyPolicy.CandidateId = (Guid)candidate.Id;
            string json = privacyPolicy.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>>((x) => x.Run(json, null));
        }
        
        private void SaveContactChannelCreation(IEnumerable<ContactChannelCreation> contactChannelCreations, Candidate candidate)
        {
            foreach (var contactChannelCreation in contactChannelCreations)
            {
                contactChannelCreation.CandidateId = (Guid)candidate.Id;
                string json = contactChannelCreation.SerializeChangeTracked();
                
                
                // FIXME: this should be applied immediately
                _jobClient.Enqueue<UpsertModelWithCandidateIdJob<ContactChannelCreation>>((x) => x.Run(json, null));
            }
        }
    }
}
