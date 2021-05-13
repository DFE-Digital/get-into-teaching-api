using FluentAssertions;
using GetIntoTeachingApi.Models;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class SchoolsExperienceSignUpTests
    {
        [Fact]
        public void Constructor_WithCandidate_MapsCorrectly()
        {
            var candidate = new Candidate()
            {
                Id = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                SecondaryPreferredTeachingSubjectId = Guid.NewGuid(),
                CountryId = Guid.NewGuid(),
                Email = "email@address.com",
                SecondaryEmail = "email2@address.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.UtcNow,
                AddressLine1 = "Address 1",
                AddressLine2 = "Address 2",
                AddressLine3 = "Address 3",
                AddressCity = "City",
                AddressStateOrProvince = "County",
                AddressPostcode = "KY11 9YU",
                AddressTelephone = "123456789",
                Telephone = "234567890",
                SecondaryTelephone = "345678901",
                MobileTelephone = "456789012",
                HasDbsCertificate = true,
                DbsCertificateIssuedAt = DateTime.UtcNow,
            };

            var response = new SchoolsExperienceSignUp(candidate);

            response.CandidateId.Should().Be(candidate.Id);
            response.PreferredTeachingSubjectId.Should().Be(candidate.PreferredTeachingSubjectId);
            response.SecondaryPreferredTeachingSubjectId.Should().Be(candidate.SecondaryPreferredTeachingSubjectId);
            response.CountryId.Should().Be(candidate.CountryId);
            response.Email.Should().Be(candidate.Email);
            response.SecondaryEmail.Should().Be(candidate.SecondaryEmail);
            response.FirstName.Should().Be(candidate.FirstName);
            response.LastName.Should().Be(candidate.LastName);
            response.AddressTelephone.Should().Be(candidate.AddressTelephone);
            response.AddressLine1.Should().Be(candidate.AddressLine1);
            response.AddressLine2.Should().Be(candidate.AddressLine2);
            response.AddressLine3.Should().Be(candidate.AddressLine3);
            response.AddressCity.Should().Be(candidate.AddressCity);
            response.AddressStateOrProvince.Should().Be(candidate.AddressStateOrProvince);
            response.AddressPostcode.Should().Be(candidate.AddressPostcode);
            response.AddressTelephone.Should().Be(candidate.AddressTelephone);
            response.Telephone.Should().Be(candidate.Telephone);
            response.SecondaryTelephone.Should().Be(candidate.SecondaryTelephone);
            response.MobileTelephone.Should().Be(candidate.MobileTelephone);
            response.HasDbsCertificate.Should().Be(candidate.HasDbsCertificate);
            response.DbsCertificateIssuedAt.Should().Be(candidate.DbsCertificateIssuedAt);
        }

        [Fact]
        public void Candidate_MapsCorrectly()
        {
            var request = new SchoolsExperienceSignUp()
            {
                CandidateId = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                SecondaryPreferredTeachingSubjectId = Guid.NewGuid(),
                CountryId = Guid.NewGuid(),
                AcceptedPolicyId = Guid.NewGuid(),
                Email = "email@address.com",
                SecondaryEmail = "email2@address.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.UtcNow,
                AddressLine1 = "Address 1",
                AddressLine2 = "Address 2",
                AddressLine3 = "Address 3",
                AddressCity = "City",
                AddressStateOrProvince = "County",
                AddressPostcode = "KY11 9YU",
                AddressTelephone = "123456789",
                Telephone = "234567890",
                SecondaryTelephone = "345678901",
                MobileTelephone = "456789012",
                HasDbsCertificate = true,
                DbsCertificateIssuedAt = DateTime.UtcNow,
            };

            var candidate = request.Candidate;

            candidate.Id.Should().Equals(request.CandidateId);
            candidate.PreferredTeachingSubjectId.Should().Equals(request.PreferredTeachingSubjectId);
            candidate.SecondaryPreferredTeachingSubjectId.Should().Equals(request.SecondaryPreferredTeachingSubjectId);
            candidate.CountryId.Should().Equals(request.CountryId);
            candidate.Email.Should().Be(request.Email);
            candidate.SecondaryEmail.Should().Be(request.SecondaryEmail);
            candidate.FirstName.Should().Be(request.FirstName);
            candidate.LastName.Should().Be(request.LastName);
            candidate.DateOfBirth.Should().Be(request.DateOfBirth);
            candidate.AddressLine1.Should().Be(request.AddressLine1);
            candidate.AddressLine2.Should().Be(request.AddressLine2);
            candidate.AddressLine3.Should().Be(request.AddressLine3);
            candidate.AddressCity.Should().Be(request.AddressCity);
            candidate.AddressStateOrProvince.Should().Be(request.AddressStateOrProvince);
            candidate.AddressPostcode.Should().Be(request.AddressPostcode);
            candidate.AddressTelephone.Should().Be(request.AddressTelephone);
            candidate.Telephone.Should().Be(request.Telephone);
            candidate.SecondaryTelephone.Should().Be(request.SecondaryTelephone);
            candidate.MobileTelephone.Should().Be(request.MobileTelephone);
            candidate.HasDbsCertificate.Should().Be(request.HasDbsCertificate);
            candidate.DbsCertificateIssuedAt.Should().Be(request.DbsCertificateIssuedAt);

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be((Guid)request.AcceptedPolicyId);
            candidate.PrivacyPolicy.AcceptedAt.Should().BeCloseTo(DateTime.UtcNow);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsSchoolsExperience()
        {
            var request = new SchoolsExperienceSignUp() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be((int)Candidate.Channel.SchoolsExperience);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNotNull_IsNotChanged()
        {
            var request = new SchoolsExperienceSignUp() { CandidateId = Guid.NewGuid() };

            request.Candidate.ChannelId.Should().BeNull();
            request.Candidate.ChangedPropertyNames.Should().NotContain("ChannelId");
        }

        [Fact]
        public void Candidate_AddressPostcode_IsFormatted()
        {
            var request = new SchoolsExperienceSignUp() { AddressPostcode = "ky119yu" };

            request.Candidate.AddressPostcode.Should().Be("KY11 9YU");
        }
    }
}
