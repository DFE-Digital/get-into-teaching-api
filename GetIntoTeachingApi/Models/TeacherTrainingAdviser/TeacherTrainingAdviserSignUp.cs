using System;
using System.Linq;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Swashbuckle.AspNetCore.Annotations;
using static GetIntoTeachingApi.Models.Crm.Candidate;

namespace GetIntoTeachingApi.Models.TeacherTrainingAdviser
{
    public class TeacherTrainingAdviserSignUp : DegreeStatusInference, ICreateContactChannel, IInferDegreeStatus
    {
        public enum ResubscribableAdviserStatus
        {
            AcceptedIttOffer = 222750000,
            AcceptedRttTeachingPosition = 222750004,
            AlreadyHasQts = 222750005,
            AskedToBeLeftAlone = 222750006,
            IncorrectlyAllocated = 222750007,
            NoLongerPursuingTeaching = 222750008,
            DoesNotWantToTrainInEngland = 222750010,
            FeHeRoute = 222750011,
            IncompleteContactDetailsUnableToTraceFurther = 222750012,
            MovedAbroad = 222750013,
            ForeignQualificationsNotEquivalent = 222750014,
            NoLongerPursuingTeachingDueToFinancialCircumstances = 222750015,
            NoLongerPursuingTeachingDueToNewEmployment = 222750016,
            NoLongerPursuingTeachingDueToPersonalCircumstances = 222750017,
            NoLongerPursuingTeachingFollowingSchoolExperience = 222750018,
            NoQts = 222750019,
            NoResponseToContactFromAdviser = 222750020,
            NonEligibleDegree = 222750021,
            NotEligibleForFunding = 222750022,
            NotEligibleForSupportFromAdviser = 222750023,
            ReferredToTeachingOutsideOfEngland = 222750024,
            NoRightToStudyInUkAndReceiveFunding = 222750025,
            WantsToApplyInFutureIttYear = 222750026,
            OfferedIttPlaceButNotTakingItUp = 222750027,
            Unsuccessful = 222750029,
            PostponingOrCancellingDueToCoronaVirusConcerns = 222750030,
            AcceptedIttTeachingPosition = 222750031,
            GainedIttPlaceBeforeAdviserAllocation = 222750034,
            SubjectClosedThisAcademicYear = 222750035,
        }

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
        public int? DegreeTypeId { get; set; }
        public int? InitialTeacherTrainingYearId { get; set; }
        public int? StageTaughtId { get; set; }
        public int? PreferredEducationPhaseId { get; set; }
        public int? HasGcseMathsAndEnglishId { get; set; }
        public int? HasGcseScienceId { get; set; }
        public int? PlanningToRetakeGcseMathsAndEnglishId { get; set; }
        public int? PlanningToRetakeGcseScienceId { get; set; }
        public int? AdviserStatusId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public int? ChannelId { get; set; }
        
        
        [SwaggerSchema(WriteOnly = true)]
        public int? CreationChannelSourceId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public int? CreationChannelServiceId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public int? CreationChannelActivityId { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [SwaggerSchema(Format = "date")]
        public DateTime? DateOfBirth { get; set; }
        public string TeacherId { get; set; }
        public string DegreeSubject { get; set; }
        public string AddressTelephone { get; set; }

        public string AddressPostcode { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public DateTime? PhoneCallScheduledAt { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool CanSubscribeToTeacherTrainingAdviser { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public int? AssignmentStatusId { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();
        [JsonIgnore]
        public IDateTimeProvider DateTimeProvider { get; set; } = new DateTimeProvider();
        [JsonIgnore]
        private bool IsOverseas => CountryId != Country.UnitedKingdomCountryId;
        
        /// <summary>
        /// Provides the default read-only contact creation channel integer value. NB: this field will be deprecated.
        /// </summary>
        public int? DefaultContactCreationChannel =>
            ChannelId ?? (int?)Candidate.Channel.TeacherTrainingAdviser; // Use the assigned channel ID if available, else assign default.

        /// <summary>
        /// Provides the default read-only creation channel source identifier.
        /// </summary>
        public int? DefaultCreationChannelSourceId =>
            (int?)ContactChannelCreation.CreationChannelSource.GITWebsite;

        /// <summary>
        /// Provides the default read-only creation channel service identifier.
        /// </summary>
        public int? DefaultCreationChannelServiceId =>
            (int?)ContactChannelCreation.CreationChannelService.TeacherTrainingAdviserService;

        /// <summary>
        /// Provides the default read-only creation channel activity identifier.
        /// </summary>
        public int? DefaultCreationChannelActivityId => null;

        /// Overrides the GraduationYear property to implement custom logic.
        /// </summary>
        public override int? GraduationYear { get; set; }

        /// <summary>
        /// Overrides the inferred graduation date to adjust its calculation.
        /// </summary>
        public override DateTime? InferredGraduationDate { get; set; }
        
        /// <summary>
        /// The situation (life stage) of the candidate, which can be used to determine their current status or context.
        /// </summary>
        public int? Situation { get; set; }

        /// <summary>
        /// The citizenship status of the candidate, represented as an integer code.
        /// </summary>
        public int? Citizenship { get; set; }

        /// <summary>
        /// The visa status of the candidate, represented as an integer code.
        /// </summary>
        public int? VisaStatus { get; set; }

        /// <summary>
        ///  The location of the candidate,represented as a string 
        /// </summary>
        public string Location { get; set; }

        public TeacherTrainingAdviserSignUp()
        {
        }

        public TeacherTrainingAdviserSignUp(Candidate candidate)
        {
            PopulateWithCandidate(candidate);
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

        private static bool CanSubscribe(Candidate candidate)
        {
            if (!candidate.HasTeacherTrainingAdviser())
            {
                return true;
            }

            if (candidate.AdviserStatusId == null)
            {
                return false;
            }

            return Enum.IsDefined(typeof(ResubscribableAdviserStatus), candidate.AdviserStatusId);
        }

        private static void DefaultPreferredEducationPhase(Candidate candidate)
        {
            if (candidate.IsReturningToTeaching() && candidate.PreferredEducationPhaseId == null)
            {
                candidate.PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary;
            }
        }

        private static void DefaultPreferredTeachingSubjectId(Candidate candidate)
        {
            if (candidate.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Primary)
            {
                candidate.PreferredTeachingSubjectId = TeachingSubject.PrimaryTeachingSubjectId;
            }
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
            AddressTelephone = candidate.AddressTelephone.StripExitCode();
            AddressPostcode = candidate.AddressPostcode;
            TypeId = candidate.TypeId;
            AdviserStatusId = candidate.AdviserStatusId;
            AssignmentStatusId = candidate.AssignmentStatusId;

            CanSubscribeToTeacherTrainingAdviser = CanSubscribe(candidate);

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
                AddressPostcode = AddressPostcode.AsFormattedPostcode(),
                AddressTelephone = AddressTelephone.AsFormattedTelephone(IsOverseas),
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

            candidate.ConfigureChannel(
                candidateId: CandidateId,
                primaryContactChannel: this);
            ConfigureGcseStatus(candidate);
            AcceptPrivacyPolicy(candidate);
            SchedulePhoneCall(candidate);
            AddQualification(candidate);
            AddPastTeachingPosition(candidate);
            SetAdviserEligibility(candidate);
            DefaultPreferredEducationPhase(candidate);
            DefaultPreferredTeachingSubjectId(candidate);
            UpdateClosedAdviserStatus(candidate);

            SubscriptionManager.SubscribeToTeacherTrainingAdviser(candidate, DateTimeProvider.UtcNow);

            return candidate;
        }

        private void UpdateClosedAdviserStatus(Candidate candidate)
        {
            // Ignore if not a closed reason.
            if (AdviserStatusId == null || !Enum.IsDefined(typeof(ResubscribableAdviserStatus), AdviserStatusId))
            {
                return;
            }

            candidate.AssignmentStatusId = (int)Candidate.AssignmentStatus.WaitingToBeAssigned;
            candidate.RegistrationStatusId = (int)Candidate.RegistrationStatus.ReRegistered;
            candidate.StatusIsWaitingToBeAssignedAt = DateTimeProvider.UtcNow;
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
                    Telephone = candidate.AddressTelephone,
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
            candidate.HasQualifiedTeacherStatus =
                candidate.IsReturningToTeaching() ?
                    (int)QualifiedTeacherStatus.Yes : (int)QualifiedTeacherStatus.No;

            // NB:  StageTaughtId is a new parameter and might not be set by older clients.
            // NB:  If the StageTaughtId==primary, SubjectTaughtId will be null
            if (candidate.IsReturningToTeaching())
            {
                if (StageTaughtId == (int)CandidatePastTeachingPosition.EducationPhase.Primary ||
                    SubjectTaughtId == TeachingSubject.PrimaryTeachingSubjectId)
                {
                    candidate.PastTeachingPositions.Add(new CandidatePastTeachingPosition()
                    {
                        Id = PastTeachingPositionId,
                        SubjectTaughtId = TeachingSubject.PrimaryTeachingSubjectId,
                        EducationPhaseId = (int)CandidatePastTeachingPosition.EducationPhase.Primary,
                    });
                }
                else if ((StageTaughtId == null ||
                          StageTaughtId == (int)CandidatePastTeachingPosition.EducationPhase.Secondary) &&
                         SubjectTaughtId != null)
                {
                    candidate.PastTeachingPositions.Add(new CandidatePastTeachingPosition()
                    {
                        Id = PastTeachingPositionId,
                        SubjectTaughtId = SubjectTaughtId,
                        EducationPhaseId = (int)CandidatePastTeachingPosition.EducationPhase.Secondary,
                    });
                }
            }
        }

        private void SetAdviserEligibility(Candidate candidate)
        {
            var eligibleForAnAdviser = PhoneCallScheduledAt == null;
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

            return IsOverseas ? (int)PhoneCall.Destination.International : (int)PhoneCall.Destination.Uk;
        }

        private bool ContainsQualification()
        {
            return UkDegreeGradeId != null || DegreeStatusId != null || DegreeSubject != null || DegreeTypeId != null;
        }
    }
}
