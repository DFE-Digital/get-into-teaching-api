using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class SchoolsExperienceSignUp
    {
        public Guid? CandidateId { get; set; }
        public Guid? PreferredTeachingSubjectId { get; set; }
        public Guid? SecondaryPreferredTeachingSubjectId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? AcceptedPolicyId { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public string FullName { get; set; }
        public string Email { get; set; }
        public string SecondaryEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [SwaggerSchema(Format = "date")]
        public DateTime? DateOfBirth { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressCity { get; set; }
        public string AddressStateOrProvince { get; set; }
        public string AddressPostcode { get; set; }
        public string AddressTelephone { get; set; }
        public string Telephone { get; set; }
        public string SecondaryTelephone { get; set; }
        public string MobileTelephone { get; set; }
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

            FullName = candidate.FullName;
            Email = candidate.Email;
            SecondaryEmail = candidate.SecondaryEmail ?? candidate.Email;
            FirstName = candidate.FirstName;
            LastName = candidate.LastName;
            DateOfBirth = candidate.DateOfBirth;
            AddressLine1 = candidate.AddressLine1;
            AddressLine2 = candidate.AddressLine2;
            AddressLine3 = candidate.AddressLine3;
            AddressCity = candidate.AddressCity;
            AddressStateOrProvince = candidate.AddressStateOrProvince;
            AddressPostcode = candidate.AddressPostcode;
            AddressTelephone = candidate.AddressTelephone;
            Telephone = candidate.Telephone;

            var secondaryTelephoneDefaults = new List<string> { candidate.MobileTelephone, candidate.AddressTelephone, candidate.Telephone };
            SecondaryTelephone = candidate.SecondaryTelephone ?? secondaryTelephoneDefaults.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));

            MobileTelephone = candidate.MobileTelephone;
            HasDbsCertificate = candidate.HasDbsCertificate;
            DbsCertificateIssuedAt = candidate.DbsCertificateIssuedAt;
        }

        private Candidate CreateCandidate()
        {
            var candidate = new Candidate()
            {
                Id = CandidateId,
                PreferredTeachingSubjectId = PreferredTeachingSubjectId,
                SecondaryPreferredTeachingSubjectId = SecondaryPreferredTeachingSubjectId,
                CountryId = LookupItem.UnitedKingdomCountryId,
                Email = Email,
                SecondaryEmail = SecondaryEmail,
                FirstName = FirstName,
                LastName = LastName,
                DateOfBirth = DateOfBirth,
                AddressLine1 = AddressLine1,
                AddressLine2 = AddressLine2,
                AddressLine3 = AddressLine3,
                AddressCity = AddressCity,
                AddressStateOrProvince = AddressStateOrProvince,
                AddressPostcode = AddressPostcode.AsFormattedPostcode(),
                AddressTelephone = AddressTelephone,
                Telephone = Telephone,
                SecondaryTelephone = SecondaryTelephone,
                MobileTelephone = MobileTelephone,
                HasDbsCertificate = HasDbsCertificate,
                DbsCertificateIssuedAt = DbsCertificateIssuedAt,
            };

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
