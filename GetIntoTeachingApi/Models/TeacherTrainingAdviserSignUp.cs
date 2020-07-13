using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
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
            HasGcseEnglishId = candidate.HasGcseEnglishId;
            HasGcseMathsId = candidate.HasGcseMathsId;
            HasGcseScienceId = candidate.HasGcseScienceId;
            PlanningToRetakeCgseScienceId = candidate.PlanningToRetakeCgseScienceId;
            PlanningToRetakeGcseEnglishId = candidate.PlanningToRetakeGcseEnglishId;
            PlanningToRetakeGcseMathsId = candidate.PlanningToRetakeGcseMathsId;

            Email = candidate.Email;
            FirstName = candidate.FirstName;
            LastName = candidate.LastName;
            DateOfBirth = candidate.DateOfBirth;
            TeacherId = candidate.TeacherId;
            Telephone = candidate.Telephone;
            AddressLine1 = candidate.AddressLine1;
            AddressLine2 = candidate.AddressLine2;
            AddressLine3 = candidate.AddressLine3;
            AddressCity = candidate.AddressCity;
            AddressState = candidate.AddressState;
            AddressPostcode = candidate.AddressPostcode;

            AlreadySubscribedToTeacherTrainingAdviser = candidate.HasActiveSubscriptionToService(Subscription.ServiceType.TeacherTrainingAdviser);

            var latestQualification = candidate.Qualifications.OrderByDescending(q => q.CreatedAt).FirstOrDefault();

            if (latestQualification != null)
            {
                QualificationId = latestQualification.Id;
                DegreeSubject = latestQualification.Subject;
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

            if (AcceptedPolicyId != null)
            {
                candidate.PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = (Guid)AcceptedPolicyId };
            }

            if (PhoneCallScheduledAt != null)
            {
                candidate.PhoneCall = new PhoneCall()
                {
                    Telephone = Telephone,
                    DestinationId = DestinationForTelephone(Telephone),
                    ScheduledAt = (DateTime)PhoneCallScheduledAt,
                    ChannelId = (int)PhoneCall.Channel.CallbackRequest,
                };
            }

            if (UkDegreeGradeId != null || DegreeStatusId != null || DegreeSubject != null || DegreeTypeId != null)
            {
                candidate.Qualifications.Add(new CandidateQualification()
                {
                    Id = QualificationId,
                    UkDegreeGradeId = UkDegreeGradeId,
                    DegreeStatusId = DegreeStatusId,
                    Subject = DegreeSubject,
                    TypeId = DegreeTypeId,
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

                candidate.PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary;
            }

            candidate.Subscriptions.Add(new Subscription() { TypeId = (int)Subscription.ServiceType.TeacherTrainingAdviser });

            return candidate;
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
    }
}
