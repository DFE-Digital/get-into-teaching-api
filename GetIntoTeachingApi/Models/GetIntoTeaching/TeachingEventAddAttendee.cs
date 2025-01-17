﻿using System;
using System.Linq;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models.GetIntoTeaching
{
    public class TeachingEventAddAttendee
    {
        public Guid? CandidateId { get; set; }
        public Guid? QualificationId { get; set; }

        [SwaggerSchema(WriteOnly = true)]
        public Guid? EventId { get; set; }

        [SwaggerSchema(WriteOnly = true)]
        public int? ChannelId { get; set; }
        
        [SwaggerSchema(WriteOnly = true)]
        public int? CreationChannelSourceId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public int? CreationChannelServiceId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public int? CreationChannelActivityId { get; set; }

        [SwaggerSchema(WriteOnly = true)]
        public Guid? AcceptedPolicyId { get; set; }
        public Guid? PreferredTeachingSubjectId { get; set; }

        public int? ConsiderationJourneyStageId { get; set; }
        public int? DegreeStatusId { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressPostcode { get; set; }
        public string AddressTelephone { get; set; }
        public bool IsVerified { get; set; } = true;
        [SwaggerSchema(WriteOnly = true)]
        public bool IsWalkIn { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public bool SubscribeToMailingList { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToEvents { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToMailingList { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToTeacherTrainingAdviser { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();
        [JsonIgnore]
        public IDateTimeProvider DateTimeProvider { get; set; } = new DateTimeProvider();

        public TeachingEventAddAttendee()
        {
        }

        public TeachingEventAddAttendee(Candidate candidate)
        {
            PopulateWithCandidate(candidate);
        }

        // We do not want to pre-fill any sign up forms if a
        // user is continuing as 'unverified'. Any fields that
        // contain personal data should be cleared, excluding
        // attributes used in match back (first/last/email).
        public void ClearAttributesForUnverifiedAccess()
        {
            AddressPostcode = null;
            AddressTelephone = null;
        }

        private void PopulateWithCandidate(Candidate candidate)
        {
            var latestQualification = candidate.Qualifications.OrderByDescending(q => q.CreatedAt).FirstOrDefault();

            if (latestQualification != null)
            {
                QualificationId = latestQualification.Id;
                DegreeStatusId = latestQualification.DegreeStatusId;
            }

            CandidateId = candidate.Id;
            PreferredTeachingSubjectId = candidate.PreferredTeachingSubjectId;

            ConsiderationJourneyStageId = candidate.ConsiderationJourneyStageId;

            Email = candidate.Email;
            FirstName = candidate.FirstName;
            LastName = candidate.LastName;
            AddressPostcode = candidate.AddressPostcode;
            AddressTelephone = candidate.AddressTelephone.StripExitCode();

            AlreadySubscribedToMailingList = candidate.HasMailingListSubscription == true;
            AlreadySubscribedToEvents = candidate.HasEventsSubscription == true;
            AlreadySubscribedToTeacherTrainingAdviser = candidate.HasTeacherTrainingAdviser();
        }

        private Candidate CreateCandidate()
        {
            var candidate = new Candidate()
            {
                Id = CandidateId,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                PreferredPhoneNumberTypeId = (int)Candidate.PhoneNumberType.Home,
                PreferredContactMethodId = (int)Candidate.ContactMethod.Any,
                GdprConsentId = (int)Candidate.GdprConsent.Consent,
                OptOutOfGdpr = false,
            };

            if (AddressPostcode != null)
            {
                candidate.AddressPostcode = AddressPostcode.AsFormattedPostcode();
            }

            if (AddressTelephone != null)
            {
                candidate.AddressTelephone = AddressTelephone;
            }

            if (ConsiderationJourneyStageId != null)
            {
                candidate.ConsiderationJourneyStageId = ConsiderationJourneyStageId;
            }

            if (PreferredTeachingSubjectId != null)
            {
                candidate.PreferredTeachingSubjectId = PreferredTeachingSubjectId;
            }

            ConfigureChannel(candidate);
            AddTeachingEventRegistration(candidate);
            AddQualification(candidate);
            AcceptPrivacyPolicy(candidate);
            ConfigureSubscriptions(candidate);

            return candidate;
        }

        private void ConfigureChannel(Candidate candidate)
        {
            if (CandidateId == null)
            {
                if (CreationChannelSourceId.HasValue)
                {
                    candidate.ChannelId = null;
                    // NB: CreationChannel should be true only if it is the first ContactChannelCreation record
                    AddCandidateCreationChannel(candidate, !candidate.ContactChannelCreations.Any());
                }
                else
                {
                    candidate.ChannelId = ChannelId ?? (int?)Candidate.Channel.Event;
                }
            }
            else // Candidate record already exists 
            {
                // NB: we do not update a candidate's ChannelId for an existing record
                // NB: CreationChannel should always be false for existing candidates
                if (CreationChannelSourceId.HasValue)
                {
                    AddCandidateCreationChannel(candidate, false);
                }
            }
        }
        
        private void AddCandidateCreationChannel(Candidate candidate, bool creationChannel)
        {
            candidate.ContactChannelCreations.Add(new ContactChannelCreation()
            {
                CreationChannel = creationChannel,
                CreationChannelSourceId = CreationChannelSourceId,
                CreationChannelServiceId = CreationChannelServiceId,
                CreationChannelActivityId = CreationChannelActivityId,
            });
        }

        private void AddQualification(Candidate candidate)
        {
            if (DegreeStatusId != null)
            {
                candidate.Qualifications.Add(new CandidateQualification()
                {
                    Id = QualificationId,
                    DegreeStatusId = DegreeStatusId,
                    TypeId = (int)CandidateQualification.DegreeType.Degree,
                });
            }
        }

        private void ConfigureSubscriptions(Candidate candidate)
        {
            var utcNow = DateTimeProvider.UtcNow;

            SubscriptionManager.SubscribeToEvents(candidate, utcNow);

            if (SubscribeToMailingList)
            {
                SubscriptionManager.SubscribeToMailingList(candidate, utcNow);
            }
        }

        private void AddTeachingEventRegistration(Candidate candidate)
        {
            if (EventId != null)
            {
                var channelId = (int)TeachingEventRegistration.Channel.Event;

                if (IsWalkIn)
                {
                    channelId = IsVerified ? (int)TeachingEventRegistration.Channel.EventWalkIn : (int)TeachingEventRegistration.Channel.EventWalkInUnverified;
                }

                candidate.TeachingEventRegistrations.Add(new TeachingEventRegistration()
                {
                    EventId = (Guid)EventId,
                    ChannelId = channelId,
                    IsCancelled = false,
                    RegistrationNotificationSeen = false,
                });
            }
        }

        private void AcceptPrivacyPolicy(Candidate candidate)
        {
            if (AcceptedPolicyId != null)
            {
                candidate.PrivacyPolicy = new CandidatePrivacyPolicy()
                {
                    AcceptedPolicyId = (Guid)AcceptedPolicyId,
                    AcceptedAt = DateTimeProvider.UtcNow,
                };
            }
        }
    }
}
