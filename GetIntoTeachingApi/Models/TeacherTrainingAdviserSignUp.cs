using System;
using System.Linq;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    [Loggable]
    public class TeacherTrainingAdviserSignUp
    {
        public Guid? CandidateId { get; set; }
        public Guid? QualificationId { get; set; }
        public Guid? SubjectTaughtId { get; set; }
        public Guid? PastTeachingPositionId { get; set; }
        public Guid? PreferredTeachingSubjectId { get; set; }
        public Guid? CountryId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? AcceptedPolicyId { get; set; }
        public int? UkDegreeGradeId { get; set; }
        public int? DegreeStatusId { get; set; }
        public int? DegreeTypeId { get; set; }
        public int? InitialTeacherTrainingYearId { get; set; }
        public int? PreferredEducationPhaseId { get; set; }
        public int? HasGcseMathsAndEnglishId { get; set; }
        public int? HasGcseScienceId { get; set; }
        public int? PlanningToRetakeGcseMathsAndEnglishId { get; set; }
        public int? PlanningToRetakeGcseScienceId { get; set; }

        [SensitiveData]
        public string Email { get; set; }
        [SensitiveData]
        public string FirstName { get; set; }
        [SensitiveData]
        public string LastName { get; set; }
        [SwaggerSchema(Format = "date")]
        [SensitiveData]
        public DateTime? DateOfBirth { get; set; }
        [SensitiveData]
        public string TeacherId { get; set; }
        public string DegreeSubject { get; set; }
        [SensitiveData]
        public string Telephone { get; set; }
        [SensitiveData]
        public string AddressLine1 { get; set; }
        [SensitiveData]
        public string AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        [SensitiveData]
        public string AddressPostcode { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public DateTime? PhoneCallScheduledAt { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToTeacherTrainingAdviser { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();

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
            Telephone = candidate.Telephone;
            AddressLine1 = candidate.AddressLine1;
            AddressLine2 = candidate.AddressLine2;
            AddressCity = candidate.AddressCity;
            AddressPostcode = candidate.AddressPostcode;

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
                AddressPostcode = AddressPostcode,
                Telephone = Telephone,
                TeacherId = TeacherId,
                InitialTeacherTrainingYearId = InitialTeacherTrainingYearId,
                PreferredEducationPhaseId = PreferredEducationPhaseId,
                HasGcseEnglishId = HasGcseMathsAndEnglishId,
                HasGcseMathsId = HasGcseMathsAndEnglishId,
                HasGcseScienceId = HasGcseScienceId,
                PlanningToRetakeGcseEnglishId = PlanningToRetakeGcseMathsAndEnglishId,
                PlanningToRetakeGcseMathsId = PlanningToRetakeGcseMathsAndEnglishId,
                PlanningToRetakeGcseScienceId = PlanningToRetakeGcseScienceId,
                ChannelId = CandidateId == null ? (int?)Candidate.Channel.TeacherTrainingAdviser : null,
                EligibilityRulesPassed = "false",
                AdviserRequirementId = null,
                AdviserEligibilityId = null,
                AssignmentStatusId = null,
            };

            ConfigureGcseStatus(candidate);
            AcceptPrivacyPolicy(candidate);
            SchedulePhoneCall(candidate);
            AddQualification(candidate);
            AddPastTeachingPosition(candidate);
            SetAdviserEligibility(candidate);
            SetType(candidate);
            DefaultPreferredEducationPhase(candidate);
            DefaultPreferredTeachingSubjectId(candidate);
            ConfigureSubscription(candidate);
            ConfigureConsent(candidate);

            return candidate;
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
            if (candidate.PreferredEducationPhaseId != (int)Candidate.PreferredEducationPhase.Primary)
            {
                return;
            }

            candidate.PreferredTeachingSubjectId = TypeEntity.PrimaryTeachingSubjectId;
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
                candidate.PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = (Guid)AcceptedPolicyId };
            }
        }

        private void SchedulePhoneCall(Candidate candidate)
        {
            if (PhoneCallScheduledAt != null)
            {
                candidate.EligibilityRulesPassed = "true";
                candidate.PhoneCall = new PhoneCall()
                {
                    Telephone = Telephone,
                    DestinationId = DestinationForTelephone(Telephone),
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

                candidate.PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary;
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
            }
        }

        private void SetType(Candidate candidate)
        {
            if (candidate.IsReturningToTeaching())
            {
                candidate.TypeId = (int)Candidate.Type.ReturningToTeacherTraining;
            }
            else
            {
                candidate.TypeId = (int)Candidate.Type.InterestedInTeacherTraining;
            }
        }

        private void ConfigureConsent(Candidate candidate)
        {
            candidate.OptOutOfSms = false;
            candidate.DoNotBulkEmail = candidate.IsReturningToTeaching();
            candidate.DoNotEmail = false;
            candidate.DoNotBulkPostalMail = candidate.IsReturningToTeaching();
            candidate.DoNotPostalMail = candidate.IsReturningToTeaching();
            candidate.DoNotSendMm = candidate.IsReturningToTeaching();
        }

        private void ConfigureSubscription(Candidate candidate)
        {
            candidate.HasTeacherTrainingAdviserSubscription = true;
            candidate.TeacherTrainingAdviserSubscriptionChannelId = (int)Candidate.SubscriptionChannel.TeacherTrainingAdviser;
            candidate.TeacherTrainingAdviserSubscriptionStartAt = DateTime.UtcNow;
            candidate.TeacherTrainingAdviserSubscriptionDoNotEmail = false;
            candidate.TeacherTrainingAdviserSubscriptionDoNotBulkEmail = candidate.IsReturningToTeaching();
            candidate.TeacherTrainingAdviserSubscriptionDoNotBulkPostalMail = candidate.IsReturningToTeaching();
            candidate.TeacherTrainingAdviserSubscriptionDoNotPostalMail = candidate.IsReturningToTeaching();
            candidate.TeacherTrainingAdviserSubscriptionDoNotSendMm = candidate.IsReturningToTeaching();
        }

        private int? DestinationForTelephone(string telephone)
        {
            if (telephone == null)
            {
                return null;
            }

            var sanitizedTelephone = telephone.Replace(" ", string.Empty);

            if (sanitizedTelephone.StartsWith("+") && !sanitizedTelephone.StartsWith("+44"))
            {
                return (int)PhoneCall.Destination.International;
            }

            return (int)PhoneCall.Destination.Uk;
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
