using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class TeacherTrainingAdviserSignUp
    {
        private string _addressTelephone;

        public Guid? CandidateId { get; set; }
        public Guid? QualificationId { get; set; }
        public Guid? SubjectTaughtId { get; set; }
        public Guid? PastTeachingPositionId { get; set; }
        public Guid? PreferredTeachingSubjectId { get; set; }
        public Guid? CountryId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? AcceptedPolicyId { get; set; }
        public int? TypeId { get; set; }
        public int? UkDegreeGradeId { get; set; }
        public int? DegreeStatusId { get; set; }
        public int? DegreeTypeId { get; set; }
        public int? InitialTeacherTrainingYearId { get; set; }
        public int? PreferredEducationPhaseId { get; set; }
        public int? HasGcseMathsAndEnglishId { get; set; }
        public int? HasGcseScienceId { get; set; }
        public int? PlanningToRetakeGcseMathsAndEnglishId { get; set; }
        public int? PlanningToRetakeGcseScienceId { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [SwaggerSchema(Format = "date")]
        public DateTime? DateOfBirth { get; set; }
        public string TeacherId { get; set; }
        public string DegreeSubject { get; set; }
        public string AddressTelephone
        {
            get
            {
                return SanitizeTelephone(_addressTelephone);
            }
            set
            {
                _addressTelephone = value;
            }
        }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressPostcode { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public DateTime? PhoneCallScheduledAt { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToTeacherTrainingAdviser { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();
        [JsonIgnore]
        public IDateTimeProvider DateTimeProvider { get; set; } = new DateTimeProvider();

        public TeacherTrainingAdviserSignUp()
        {
        }

        public TeacherTrainingAdviserSignUp(Candidate candidate)
        {
            PopulateWithCandidate(candidate);
        }

        private void PopulateWithCandidate(Candidate candidate)
        {
            CandidateId = candidate.Id;
            PreferredTeachingSubjectId = candidate.PreferredTeachingSubjectId;
            CountryId = candidate.CountryId;

            InitialTeacherTrainingYearId = candidate.InitialTeacherTrainingYearId;
            PreferredEducationPhaseId = candidate.PreferredEducationPhaseId;
            HasGcseScienceId = candidate.HasGcseScienceId;
            PlanningToRetakeGcseScienceId = candidate.PlanningToRetakeGcseScienceId;

            if (candidate.HasGcseMathsAndEnglish())
            {
                HasGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking;
            }

            if (candidate.IsPlanningToRetakeGcseMathsAndEnglish())
            {
                PlanningToRetakeGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking;
            }

            Email = candidate.Email;
            FirstName = candidate.FirstName;
            LastName = candidate.LastName;
            DateOfBirth = candidate.DateOfBirth;
            TeacherId = candidate.TeacherId;
            AddressTelephone = candidate.AddressTelephone;
            AddressLine1 = candidate.AddressLine1;
            AddressLine2 = candidate.AddressLine2;
            AddressCity = candidate.AddressCity;
            AddressPostcode = candidate.AddressPostcode;
            TypeId = candidate.TypeId;

            AlreadySubscribedToTeacherTrainingAdviser = candidate.HasTeacherTrainingAdviser();

            var latestQualification = candidate.Qualifications.OrderByDescending(q => q.CreatedAt).FirstOrDefault();

            if (latestQualification != null)
            {
                QualificationId = latestQualification.Id;
                DegreeSubject = latestQualification.DegreeSubject;
                UkDegreeGradeId = latestQualification.UkDegreeGradeId;
                DegreeStatusId = latestQualification.DegreeStatusId;
                DegreeTypeId = latestQualification.TypeId;
            }

            var latestPastTeachingPosition = candidate.PastTeachingPositions.OrderByDescending(q => q.CreatedAt).FirstOrDefault();

            if (latestPastTeachingPosition != null)
            {
                PastTeachingPositionId = latestPastTeachingPosition.Id;
                SubjectTaughtId = latestPastTeachingPosition.SubjectTaughtId;
            }
        }

        private Candidate CreateCandidate()
        {
            var candidate = new Candidate()
            {
                Id = CandidateId,
                PreferredTeachingSubjectId = PreferredTeachingSubjectId,
                CountryId = CountryId,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                DateOfBirth = DateOfBirth,
                AddressLine1 = AddressLine1,
                AddressLine2 = AddressLine2,
                AddressCity = AddressCity,
                AddressPostcode = AddressPostcode.AsFormattedPostcode(),
                AddressTelephone = AddressTelephone,
                TeacherId = TeacherId,
                TypeId = TypeId,
                InitialTeacherTrainingYearId = InitialTeacherTrainingYearId,
                PreferredEducationPhaseId = PreferredEducationPhaseId,
                HasGcseEnglishId = HasGcseMathsAndEnglishId,
                HasGcseMathsId = HasGcseMathsAndEnglishId,
                HasGcseScienceId = HasGcseScienceId,
                PlanningToRetakeGcseEnglishId = PlanningToRetakeGcseMathsAndEnglishId,
                PlanningToRetakeGcseMathsId = PlanningToRetakeGcseMathsAndEnglishId,
                PlanningToRetakeGcseScienceId = PlanningToRetakeGcseScienceId,
                EligibilityRulesPassed = "false",
                AdviserRequirementId = null,
                AdviserEligibilityId = null,
                AssignmentStatusId = null,
                PreferredPhoneNumberTypeId = (int)Candidate.PhoneNumberType.Home,
                PreferredContactMethodId = (int)Candidate.ContactMethod.Any,
                GdprConsentId = (int)Candidate.GdprConsent.Consent,
                OptOutOfGdpr = false,
            };

            ConfigureChannel(candidate);
            ConfigureGcseStatus(candidate);
            AcceptPrivacyPolicy(candidate);
            SchedulePhoneCall(candidate);
            AddQualification(candidate);
            AddPastTeachingPosition(candidate);
            SetAdviserEligibility(candidate);
            DefaultPreferredEducationPhase(candidate);
            DefaultPreferredTeachingSubjectId(candidate);
            SubscriptionManager.SubscribeToTeacherTrainingAdviser(candidate, DateTimeProvider.UtcNow);

            return candidate;
        }

        private string SanitizeTelephone(string telephone)
        {
            var international = CountryId != LookupItem.UnitedKingdomCountryId;

            if (international && telephone != null)
            {
                // Remove non-digit characters.
                telephone = Regex.Replace(telephone, "[^0-9]", string.Empty);

                // Prefix the 00 exit code.
                telephone = $"00{telephone}";

                // Replace UK dial-in code with a 0.
                telephone = Regex.Replace(telephone, "^00440?", "0");
            }

            return telephone;
        }

        private void ConfigureChannel(Candidate candidate)
        {
            if (CandidateId == null)
            {
                candidate.ChannelId = (int?)Candidate.Channel.TeacherTrainingAdviser;
            }
        }

        private void DefaultPreferredEducationPhase(Candidate candidate)
        {
            if (candidate.IsReturningToTeaching())
            {
                candidate.PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary;
            }
        }

        private void DefaultPreferredTeachingSubjectId(Candidate candidate)
        {
            if (candidate.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Primary)
            {
                candidate.PreferredTeachingSubjectId = LookupItem.PrimaryTeachingSubjectId;
            }
        }

        private void ConfigureGcseStatus(Candidate candidate)
        {
            if (HasGcseMathsAndEnglishId == null)
            {
                candidate.HasGcseMathsId = (int)Candidate.GcseStatus.NotAnswered;
                candidate.HasGcseEnglishId = (int)Candidate.GcseStatus.NotAnswered;
            }

            if (HasGcseScienceId == null)
            {
                candidate.HasGcseScienceId = (int)Candidate.GcseStatus.NotAnswered;
            }

            if (PlanningToRetakeGcseMathsAndEnglishId == null)
            {
                candidate.PlanningToRetakeGcseMathsId = (int)Candidate.GcseStatus.NotAnswered;
                candidate.PlanningToRetakeGcseEnglishId = (int)Candidate.GcseStatus.NotAnswered;
            }

            if (PlanningToRetakeGcseScienceId == null)
            {
                candidate.PlanningToRetakeGcseScienceId = (int)Candidate.GcseStatus.NotAnswered;
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

        private void SchedulePhoneCall(Candidate candidate)
        {
            if (PhoneCallScheduledAt != null)
            {
                candidate.EligibilityRulesPassed = "true";
                candidate.PhoneCall = new PhoneCall()
                {
                    Telephone = AddressTelephone,
                    DestinationId = TelephoneDestination(),
                    ScheduledAt = (DateTime)PhoneCallScheduledAt,
                    ChannelId = (int)PhoneCall.Channel.CallbackRequest,
                    Subject = $"Scheduled phone call requested by {candidate.FullName}",
                };
            }
        }

        private void AddQualification(Candidate candidate)
        {
            if (ContainsQualification())
            {
                candidate.Qualifications.Add(new CandidateQualification()
                {
                    Id = QualificationId,
                    UkDegreeGradeId = UkDegreeGradeId,
                    DegreeStatusId = DegreeStatusId,
                    DegreeSubject = DegreeSubject,
                    TypeId = DegreeTypeId,
                });
            }
        }

        private void AddPastTeachingPosition(Candidate candidate)
        {
            if (ContainsPastTeachingPosition())
            {
                candidate.PastTeachingPositions.Add(new CandidatePastTeachingPosition()
                {
                    Id = PastTeachingPositionId,
                    SubjectTaughtId = SubjectTaughtId,
                    EducationPhaseId = (int)CandidatePastTeachingPosition.EducationPhase.Secondary,
                });
            }
        }

        private void SetAdviserEligibility(Candidate candidate)
        {
            var eligibleForAnAdviser = DegreeTypeId == (int)CandidateQualification.DegreeType.Degree || candidate.IsReturningToTeaching();
            if (eligibleForAnAdviser)
            {
                candidate.AssignmentStatusId = (int)Candidate.AssignmentStatus.WaitingToBeAssigned;
                candidate.AdviserEligibilityId = (int)Candidate.AdviserEligibility.Yes;
                candidate.AdviserRequirementId = (int)Candidate.AdviserRequirement.Yes;
                candidate.StatusIsWaitingToBeAssignedAt = DateTimeProvider.UtcNow;
            }
        }

        private int? TelephoneDestination()
        {
            if (AddressTelephone == null)
            {
                return null;
            }

            var international = CountryId != LookupItem.UnitedKingdomCountryId;

            return international ? (int)PhoneCall.Destination.International : (int)PhoneCall.Destination.Uk;
        }

        private bool ContainsQualification()
        {
            return UkDegreeGradeId != null || DegreeStatusId != null || DegreeSubject != null || DegreeTypeId != null;
        }

        private bool ContainsPastTeachingPosition()
        {
            return SubjectTaughtId != null;
        }
    }
}
