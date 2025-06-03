using System;
using System.Linq;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models.SchoolsExperience
{
    public class SchoolsExperienceSignUp : ICreateContactChannel
    {
        public Guid? CandidateId { get; set; }
        public Guid? PreferredTeachingSubjectId { get; set; }
        public Guid? SecondaryPreferredTeachingSubjectId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? AcceptedPolicyId { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public Guid? MasterId { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public bool Merged { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public string FullName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressCity { get; set; }
        public string AddressStateOrProvince { get; set; }
        public string AddressPostcode { get; set; }
        public string Telephone { get; set; }
        public bool? HasDbsCertificate { get; set; }
        public DateTime? DbsCertificateIssuedAt { get; set; }
        public Guid? QualificationId { get; set; }
        public int? DegreeStatusId { get; set; }
        public int? DegreeTypeId { get; set; }
        public string DegreeSubject { get; set; }
        public int? UkDegreeGradeId { get; set; }
        
        [SwaggerSchema(WriteOnly = true)]
        public int? CreationChannelSourceId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public int? CreationChannelServiceId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public int? CreationChannelActivityId { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();
        [JsonIgnore]
        public IDateTimeProvider DateTimeProvider { get; set; } = new DateTimeProvider();
        
        /// <summary>
        /// Provides the default read-only contact creation channel integer value. NB: this field will be deprecated.
        /// </summary>
        public int? DefaultContactCreationChannel =>
            (int?)Candidate.Channel.SchoolsExperience;

        /// <summary>
        /// Provides the default read-only creation channel source identifier.
        /// </summary>
        public int? DefaultCreationChannelSourceId =>
            (int?)ContactChannelCreation.CreationChannelSource.SchoolExperience;

        /// <summary>
        /// Provides the default read-only creation channel service identifier.
        /// </summary>
        public int? DefaultCreationChannelServiceId =>
            (int?)ContactChannelCreation.CreationChannelService.CreatedOnSchoolExperience;

        /// <summary>
        /// Provides the default read-only creation channel activity identifier.
        /// </summary>
        public int? DefaultCreationChannelActivityId => null;

        public SchoolsExperienceSignUp(){
        }

        public SchoolsExperienceSignUp(Candidate candidate)
        {
            PopulateWithCandidate(candidate);
        }

        private void PopulateWithCandidate(Candidate candidate)
        {
            CandidateId = candidate.Id;
            PreferredTeachingSubjectId = candidate.PreferredTeachingSubjectId;
            SecondaryPreferredTeachingSubjectId = candidate.SecondaryPreferredTeachingSubjectId;
            MasterId = candidate.MasterId;

            Merged = candidate.Merged;
            FullName = candidate.FullName;
            Email = candidate.Email;
            FirstName = candidate.FirstName;
            LastName = candidate.LastName;
            AddressLine1 = candidate.AddressLine1;
            AddressLine2 = candidate.AddressLine2;
            AddressLine3 = candidate.AddressLine3;
            AddressCity = candidate.AddressCity;
            AddressStateOrProvince = candidate.AddressStateOrProvince;
            AddressPostcode = candidate.AddressPostcode;

            // We only want to expose primary phone number to School Experience. However, there
            // are a number of records in the CRM that have School Experience but no primary number.
            // Fetch the other phone numbers for now until the issue has been resolved in the CRM.
            var telephoneReserves = new string[] { candidate.Telephone, candidate.MobileTelephone, candidate.SecondaryTelephone };
            Telephone = candidate.AddressTelephone.StripExitCode() ?? Array.Find(telephoneReserves, number => !string.IsNullOrWhiteSpace(number)).StripExitCode();

            HasDbsCertificate = candidate.HasDbsCertificate;
            DbsCertificateIssuedAt = candidate.DbsCertificateIssuedAt;
            
            var latestQualification = candidate.Qualifications.OrderByDescending(q => q.CreatedAt).FirstOrDefault();
            if (latestQualification != null)
            {
                QualificationId = latestQualification.Id;
                DegreeSubject = latestQualification.DegreeSubject;
                UkDegreeGradeId = latestQualification.UkDegreeGradeId;
                DegreeStatusId = latestQualification.DegreeStatusId;
                DegreeTypeId = latestQualification.TypeId;
            }
            
            var latestContactChannelCreation = candidate.ContactChannelCreations.OrderByDescending(c => c.CreatedAt).FirstOrDefault();
            if (latestContactChannelCreation != null)
            {
                CreationChannelSourceId = latestContactChannelCreation.CreationChannelSourceId;
                CreationChannelServiceId = latestContactChannelCreation.CreationChannelServiceId;
                CreationChannelActivityId = latestContactChannelCreation.CreationChannelActivityId;
            }
        }

        private Candidate CreateCandidate()
        {
            var candidate = new Candidate()
            {
                Id = CandidateId,
                CountryId = Country.UnitedKingdomCountryId,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                AddressLine1 = AddressLine1,
                AddressLine2 = AddressLine2,
                AddressLine3 = AddressLine3,
                AddressCity = AddressCity,
                AddressStateOrProvince = AddressStateOrProvince,
                AddressPostcode = AddressPostcode.AsFormattedPostcode(),
                AddressTelephone = Telephone,
                HasDbsCertificate = HasDbsCertificate,
                DbsCertificateIssuedAt = DbsCertificateIssuedAt,
            };

            if (PreferredTeachingSubjectId != null)
            {
                candidate.PreferredTeachingSubjectId = PreferredTeachingSubjectId;
            }

            if (SecondaryPreferredTeachingSubjectId != null)
            {
                candidate.SecondaryPreferredTeachingSubjectId = SecondaryPreferredTeachingSubjectId;
            }

            // Ensures we don't create duplicate ContactCreationChannel records
            if (ShouldCreateSchoolExperienceCreationChannel(candidate))
            {
                candidate.ConfigureChannel(contactChannelCreator: this, candidateId: CandidateId);
            }

            AcceptPrivacyPolicy(candidate);
            AddQualification(candidate);

            return candidate;
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
        
        private void AddQualification(Candidate candidate)
        {
            if (ContainsQualification() && candidate.Qualifications.Count == 0)
            {
                candidate.Qualifications.Add(new CandidateQualification()
                {
                    Id = QualificationId,
                    UkDegreeGradeId = UkDegreeGradeId,
                    DegreeStatusId = DegreeStatusId,
                    DegreeSubject = DegreeSubject,
                    TypeId = DegreeTypeId ?? (int)CandidateQualification.DegreeType.Degree
                });
            }
        }
        
        private bool ContainsQualification()
        {
            return UkDegreeGradeId != null || DegreeStatusId != null || DegreeSubject != null || DegreeTypeId != null;
        }

        private bool ShouldCreateSchoolExperienceCreationChannel(Candidate candidate)
        {
            if (CreationChannelSourceId != null)
            {
                var latestContactChannelCreation = candidate.ContactChannelCreations.OrderByDescending(c => c.CreatedAt).FirstOrDefault();

                if (latestContactChannelCreation != null &&
                    latestContactChannelCreation.CreationChannelSourceId == CreationChannelSourceId)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
