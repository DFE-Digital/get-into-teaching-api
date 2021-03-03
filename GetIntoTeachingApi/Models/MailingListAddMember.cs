using System;
using System.Linq;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class MailingListAddMember
    {
        public Guid? CandidateId { get; set; }
        public Guid? QualificationId { get; set; }
        public Guid? PreferredTeachingSubjectId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? AcceptedPolicyId { get; set; }

        public int? ConsiderationJourneyStageId { get; set; }
        public int? DegreeStatusId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public int? ChannelId { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressPostcode { get; set; }
        public string Telephone { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToEvents { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToMailingList { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToTeacherTrainingAdviser { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();

        public MailingListAddMember()
        {
        }

        public MailingListAddMember(Candidate candidate)
        {
            PopulateWithCandidate(candidate);
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
            Telephone = candidate.Telephone;

            AlreadySubscribedToMailingList = candidate.HasMailingListSubscription == true;
            AlreadySubscribedToEvents = candidate.HasEventsSubscription == true;
            AlreadySubscribedToTeacherTrainingAdviser = candidate.HasTeacherTrainingAdviser();
        }

        private Candidate CreateCandidate()
        {
            var candidate = new Candidate()
            {
                Id = CandidateId,
                ConsiderationJourneyStageId = ConsiderationJourneyStageId,
                PreferredTeachingSubjectId = PreferredTeachingSubjectId,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                AddressPostcode = AddressPostcode,
                Telephone = Telephone,
                EligibilityRulesPassed = "false",
            };

            ConfigureChannel(candidate);
            ConfigureSubscriptions(candidate);
            AddQualification(candidate);
            AcceptPrivacyPolicy(candidate);

            return candidate;
        }

        private void ConfigureChannel(Candidate candidate)
        {
            if (CandidateId == null)
            {
                candidate.ChannelId = ChannelId ?? (int?)Candidate.Channel.MailingList;
            }
        }

        private void AddQualification(Candidate candidate)
        {
            candidate.Qualifications.Add(new CandidateQualification()
            {
                Id = QualificationId,
                DegreeStatusId = DegreeStatusId,
                TypeId = (int)CandidateQualification.DegreeType.Degree,
            });
        }

        private void ConfigureSubscriptions(Candidate candidate)
        {
            SubscriptionManager.SubscribeToMailingList(candidate, ChannelId);

            if (!string.IsNullOrWhiteSpace(AddressPostcode))
            {
                SubscriptionManager.SubscribeToEvents(candidate, ChannelId);
            }
        }

        private void AcceptPrivacyPolicy(Candidate candidate)
        {
            if (AcceptedPolicyId != null)
            {
                candidate.PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = (Guid)AcceptedPolicyId };
            }
        }
    }
}
