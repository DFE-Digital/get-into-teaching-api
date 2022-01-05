using System;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models.SchoolsExperience
{
    public class SchoolsExperienceSignUp
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

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();
        [JsonIgnore]
        public IDateTimeProvider DateTimeProvider { get; set; } = new DateTimeProvider();

        public SchoolsExperienceSignUp()
        {
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
        }

        private Candidate CreateCandidate()
        {
            var candidate = new Candidate()
            {
                Id = CandidateId,
                CountryId = LookupItem.UnitedKingdomCountryId,
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

            ConfigureChannel(candidate);
            AcceptPrivacyPolicy(candidate);

            return candidate;
        }

        private void ConfigureChannel(Candidate candidate)
        {
            if (CandidateId == null)
            {
                candidate.ChannelId = (int?)Candidate.Channel.SchoolsExperience;
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
    }
}
