using System;
using System.Text.Json.Serialization;

namespace GetIntoTeachingApi.Models
{
    public class MailingListAddMemberRequest
    {
        public Guid? CandidateId { get; set; }
        public Guid PreferredTeachingSubjectId { get; set; }
        public Guid AcceptedPolicyId { get; set; }

        public int DescribeYourselfOptionId { get; set; }
        public int ConsiderationJourneyStageId { get; set; }
        public int UkDegreeGradeId { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressPostcode { get; set; }
        public string Telephone { get; set; }
        public string CallbackInformation { get; set; }
        public bool SubscribeToEvents { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();

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
                PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = AcceptedPolicyId },
                ChannelId = CandidateId == null ? (int?)Candidate.Channel.MailingList : null,
                EligibilityRulesPassed = Telephone == null ? "false" : "true",
                OptOutOfSms = false,
                DoNotBulkEmail = false,
                DoNotEmail = false,
                DoNotBulkPostalMail = true,
                DoNotPostalMail = true,
                DoNotSendMm = false,
            };

            candidate.Qualifications.Add(new CandidateQualification() { UkDegreeGradeId = UkDegreeGradeId });
            candidate.Subscriptions.Add(new Subscription() { TypeId = (int)Subscription.ServiceType.MailingList });

            if (SubscribeToEvents)
            {
                candidate.Subscriptions.Add(new Subscription() { TypeId = (int)Subscription.ServiceType.Event });
            }

            return candidate;
        }
    }
}
