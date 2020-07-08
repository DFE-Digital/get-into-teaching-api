using System;
using System.Text.Json.Serialization;

namespace GetIntoTeachingApi.Models
{
    public class TeacherTrainingAdviserSignUpRequest
    {
        public Guid? CandidateId { get; set; }
        public Guid? QualificationId { get; set; }
        public Guid? SubjectTaughtId { get; set; }
        public Guid? PastTeachingPositionId { get; set; }
        public Guid? PreferredTeachingSubjectId { get; set; }
        public Guid? CountryId { get; set; }
        public Guid AcceptedPolicyId { get; set; }

        public int? UkDegreeGradeId { get; set; }
        public int? DegreeStatusId { get; set; }
        public int? InitialTeacherTrainingYearId { get; set; }
        public int? PreferredEducationPhaseId { get; set; }
        public int? HasGcseEnglishId { get; set; }
        public int? HasGcseMathsId { get; set; }
        public int? HasGcseScienceId { get; set; }
        public int? PlanningToRetakeGcseEnglishId { get; set; }
        public int? PlanningToRetakeGcseMathsId { get; set; }
        public int? PlanningToRetakeCgseScienceId { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string TeacherId { get; set; }
        public string DegreeSubject { get; set; }
        public string Telephone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressPostcode { get; set; }
        public DateTime? PhoneCallScheduledAt { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();

        private Candidate CreateCandidate()
        {
            var candidate = new Candidate()
            {
                Id = CandidateId,
                PreferredTeachingSubjectId = PreferredTeachingSubjectId,
                CountryId = CountryId,
                PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = AcceptedPolicyId },
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                DateOfBirth = DateOfBirth,
                AddressLine1 = AddressLine1,
                AddressLine2 = AddressLine2,
                AddressLine3 = AddressLine3,
                AddressState = AddressState,
                AddressCity = AddressCity,
                AddressPostcode = AddressPostcode,
                Telephone = Telephone,
                TeacherId = TeacherId,
                InitialTeacherTrainingYearId = InitialTeacherTrainingYearId,
                PreferredEducationPhaseId = PreferredEducationPhaseId,
                HasGcseEnglishId = HasGcseEnglishId,
                HasGcseMathsId = HasGcseMathsId,
                HasGcseScienceId = HasGcseScienceId,
                PlanningToRetakeGcseEnglishId = PlanningToRetakeGcseEnglishId,
                PlanningToRetakeGcseMathsId = PlanningToRetakeGcseMathsId,
                PlanningToRetakeCgseScienceId = PlanningToRetakeCgseScienceId,
                ChannelId = CandidateId == null ? (int?)Candidate.Channel.TeacherTrainingAdviser : null,
                EligibilityRulesPassed = "false",
                AdviserRequirementId = null,
                AdviserEligibilityId = null,
                StatusId = null,
                OptOutOfSms = false,
                DoNotBulkEmail = false,
                DoNotEmail = false,
                DoNotBulkPostalMail = false,
                DoNotPostalMail = false,
                DoNotSendMm = false,
            };

            if (PhoneCallScheduledAt != null)
            {
                candidate.PhoneCall = new PhoneCall()
                {
                    Telephone = Telephone,
                    ScheduledAt = (DateTime)PhoneCallScheduledAt,
                    ChannelId = (int)PhoneCall.Channel.CallbackRequest,
                };
            }

            if (UkDegreeGradeId != null || DegreeStatusId != null || DegreeSubject != null)
            {
                candidate.Qualifications.Add(new CandidateQualification()
                {
                    Id = QualificationId,
                    UkDegreeGradeId = UkDegreeGradeId,
                    DegreeStatusId = DegreeStatusId,
                    Subject = DegreeSubject,
                });
            }

            if (SubjectTaughtId != null)
            {
                candidate.PastTeachingPositions.Add(new CandidatePastTeachingPosition()
                {
                    Id = PastTeachingPositionId,
                    SubjectTaughtId = SubjectTaughtId,
                    EducationPhaseId = (int)CandidatePastTeachingPosition.EducationPhase.Secondary,
                });
            }

            candidate.Subscriptions.Add(new Subscription() { TypeId = (int)Subscription.ServiceType.TeacherTrainingAdviser });

            return candidate;
        }
    }
}
