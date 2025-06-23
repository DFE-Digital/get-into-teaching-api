using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace GetIntoTeachingApi.Models.GetIntoTeaching
{
    public class MailingListAddMember : DegreeStatusInference, IInferDegreeStatus
    {
        public Guid? CandidateId { get; set; }
        public Guid? QualificationId { get; set; }
        public Guid? PreferredTeachingSubjectId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? AcceptedPolicyId { get; set; }

        public int? ConsiderationJourneyStageId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public int? ChannelId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressPostcode { get; set; }
        public string WelcomeGuideVariant { get; set; }
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

        public int? Situation { get; set; }

        /// <summary>
        /// Overrides the GraduationYear property to implement custom logic.
        /// </summary>
        public override int? GraduationYear{ get; set; }

        /// <summary>
        /// Overrides the inferred graduation date to adjust its calculation.
        /// </summary>
        public override DateTime? InferredGraduationDate { get; set; }

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
            WelcomeGuideVariant = candidate.WelcomeGuideVariant;
            Situation = candidate.Situation;

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
                AddressPostcode = AddressPostcode.AsFormattedPostcode(),
                WelcomeGuideVariant = WelcomeGuideVariant,
                EligibilityRulesPassed = "false",
                PreferredPhoneNumberTypeId = (int)Candidate.PhoneNumberType.Home,
                PreferredContactMethodId = (int)Candidate.ContactMethod.Any,
                GdprConsentId = (int)Candidate.GdprConsent.Consent,
                OptOutOfGdpr = false,
                Situation = Situation
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
                GraduationYear = InferredGraduationDate
            });
        }

        private void ConfigureSubscriptions(Candidate candidate)
        {
            var utcNow = DateTimeProvider.UtcNow;

            SubscriptionManager.SubscribeToMailingList(candidate, utcNow);

            if (!string.IsNullOrWhiteSpace(AddressPostcode))
            {
                SubscriptionManager.SubscribeToEvents(candidate, utcNow);
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

        /// <summary>
        /// Provides logic to conditionally infer the degree status based on
        /// the graduation year, if the graduation year is provisioned.
        /// </summary>
        /// <param name="degreeStatusDomainService">
        /// Implementation of <see cref="IDegreeStatusDomainService"/> which
        /// provides the functionality for inferring degree status based on the proposed graduation year.
        /// </param>
        /// <param name="currentYearProvider">
        /// Implementation of <see cref="ICurrentYearProvider"/> which provides the current date/time
        /// as well as helper methods to convert the current year to an integer representation.
        /// </param>
        public int? InferDegreeStatus(
            IDegreeStatusDomainService degreeStatusDomainService,
            ICurrentYearProvider currentYearProvider) =>
                GetInferredDegreeStatus(degreeStatusDomainService, currentYearProvider);
    }
}
