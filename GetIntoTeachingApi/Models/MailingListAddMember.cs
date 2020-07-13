using System;
using System.Linq;
using System.Text.Json.Serialization;
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

        public int? DescribeYourselfOptionId { get; set; }
        public int? ConsiderationJourneyStageId { get; set; }
        public int? UkDegreeGradeId { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressPostcode { get; set; }
        public string Telephone { get; set; }
        public string CallbackInformation { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public bool SubscribeToEvents { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToEvents { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToMailingList { get; set; }

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
                UkDegreeGradeId = latestQualification.UkDegreeGradeId;
            }

            CandidateId = candidate.Id;
            PreferredTeachingSubjectId = candidate.PreferredTeachingSubjectId;

            DescribeYourselfOptionId = candidate.DescribeYourselfOptionId;
            ConsiderationJourneyStageId = candidate.ConsiderationJourneyStageId;

            Email = candidate.Email;
            FirstName = candidate.FirstName;
            LastName = candidate.LastName;
            AddressPostcode = candidate.AddressPostcode;
            Telephone = candidate.Telephone;
            CallbackInformation = candidate.CallbackInformation;

            AlreadySubscribedToMailingList = candidate.HasActiveSubscriptionToService(Subscription.ServiceType.MailingList);
            AlreadySubscribedToEvents = candidate.HasActiveSubscriptionToService(Subscription.ServiceType.Event);
        }

        private Candidate CreateCandidate()
        {
            var candidate = new Candidate()
            {
                Id = CandidateId,
                DescribeYourselfOptionId = DescribeYourselfOptionId,
                ConsiderationJourneyStageId = ConsiderationJourneyStageId,
                PreferredTeachingSubjectId = PreferredTeachingSubjectId,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                AddressPostcode = AddressPostcode,
                Telephone = Telephone,
                CallbackInformation = CallbackInformation,
                ChannelId = CandidateId == null ? (int?)Candidate.Channel.MailingList : null,
                EligibilityRulesPassed = "false",
                OptOutOfSms = false,
                DoNotBulkEmail = false,
                DoNotEmail = false,
                DoNotBulkPostalMail = true,
                DoNotPostalMail = true,
                DoNotSendMm = false,
            };

            candidate.Qualifications.Add(new CandidateQualification() { Id = QualificationId, UkDegreeGradeId = UkDegreeGradeId });
            candidate.Subscriptions.Add(new Subscription() { TypeId = (int)Subscription.ServiceType.MailingList });

            if (AcceptedPolicyId != null)
            {
                candidate.PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = (Guid)AcceptedPolicyId };
            }

            if (SubscribeToEvents)
            {
                candidate.Subscriptions.Add(new Subscription() { TypeId = (int)Subscription.ServiceType.Event });
            }

            return candidate;
        }
    }
}
