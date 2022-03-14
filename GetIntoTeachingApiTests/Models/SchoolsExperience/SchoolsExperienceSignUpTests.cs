using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.SchoolsExperience;
using System;
using System.Reflection;
using Xunit;

namespace GetIntoTeachingApiTests.Models.SchoolsExperience
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
                MasterId = Guid.NewGuid(),
                Merged = true,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.UtcNow,
                AddressLine1 = "Address 1",
                AddressLine2 = "Address 2",
                AddressLine3 = "Address 3",
                AddressCity = "City",
                AddressStateOrProvince = "County",
                AddressPostcode = "KY11 9YU",
                Telephone = "00234567890",
                HasDbsCertificate = true,
                DbsCertificateIssuedAt = DateTime.UtcNow,
            };

            var response = new SchoolsExperienceSignUp(candidate);

            response.CandidateId.Should().Be(candidate.Id);
            response.PreferredTeachingSubjectId.Should().Be(candidate.PreferredTeachingSubjectId);
            response.SecondaryPreferredTeachingSubjectId.Should().Be(candidate.SecondaryPreferredTeachingSubjectId);
            response.MasterId.Should().Be(candidate.MasterId);
            response.Email.Should().Be(candidate.Email);
            response.Merged.Should().Be(candidate.Merged);
            response.FullName.Should().Be(candidate.FullName);
            response.FirstName.Should().Be(candidate.FirstName);
            response.LastName.Should().Be(candidate.LastName);
            response.AddressLine1.Should().Be(candidate.AddressLine1);
            response.AddressLine2.Should().Be(candidate.AddressLine2);
            response.AddressLine3.Should().Be(candidate.AddressLine3);
            response.AddressCity.Should().Be(candidate.AddressCity);
            response.AddressStateOrProvince.Should().Be(candidate.AddressStateOrProvince);
            response.AddressPostcode.Should().Be(candidate.AddressPostcode);
            response.Telephone.Should().Be(candidate.Telephone[2..]);
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
                AcceptedPolicyId = Guid.NewGuid(),
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                AddressLine1 = "Address 1",
                AddressLine2 = "Address 2",
                AddressLine3 = "Address 3",
                AddressCity = "City",
                AddressStateOrProvince = "County",
                AddressPostcode = "KY11 9YU",
                Telephone = "234567890",
                HasDbsCertificate = true,
                DbsCertificateIssuedAt = DateTime.UtcNow,
            };

            var candidate = request.Candidate;

            candidate.Id.Should().Be(request.CandidateId);
            candidate.PreferredTeachingSubjectId.Should().Be(request.PreferredTeachingSubjectId);
            candidate.SecondaryPreferredTeachingSubjectId.Should().Be(request.SecondaryPreferredTeachingSubjectId);
            candidate.CountryId.Should().Be(LookupItem.UnitedKingdomCountryId);
            candidate.Email.Should().Be(request.Email);
            candidate.FirstName.Should().Be(request.FirstName);
            candidate.LastName.Should().Be(request.LastName);
            candidate.AddressLine1.Should().Be(request.AddressLine1);
            candidate.AddressLine2.Should().Be(request.AddressLine2);
            candidate.AddressLine3.Should().Be(request.AddressLine3);
            candidate.AddressCity.Should().Be(request.AddressCity);
            candidate.AddressStateOrProvince.Should().Be(request.AddressStateOrProvince);
            candidate.AddressPostcode.Should().Be(request.AddressPostcode);
            candidate.AddressTelephone.Should().Be(request.Telephone);
            candidate.HasDbsCertificate.Should().Be(request.HasDbsCertificate);
            candidate.DbsCertificateIssuedAt.Should().Be(request.DbsCertificateIssuedAt);

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be((Guid)request.AcceptedPolicyId);
            candidate.PrivacyPolicy.AcceptedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
        }

        [Theory]
        [InlineData(nameof(Candidate.Telephone))]
        [InlineData(nameof(Candidate.MobileTelephone))]
        [InlineData(nameof(Candidate.SecondaryTelephone))]
        public void Constructor_WhenCandidateHasNoPrimaryTelephone_UsesReserveNumbers(string reserveNumber)
        {
            var candidate = new Candidate();
            var reserveNumberProperty = typeof(Candidate).GetProperty(reserveNumber);
            reserveNumberProperty.SetValue(candidate, "07123456789");

            var response = new SchoolsExperienceSignUp(candidate);

            response.Telephone.Should().Be((string)reserveNumberProperty.GetValue(candidate));
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

        [Fact]
        public void Candidate_WhenPreferredTeachingSubjectIdIsNull_DoesNotChange()
        {
            var request = new SchoolsExperienceSignUp() { PreferredTeachingSubjectId = null };

            request.Candidate.ChangedPropertyNames.Should().NotContain("PreferredTeachingSubjectId");
        }

        [Fact]
        public void Candidate_WhenSecondaryPreferredTeachingSubjectIdIsNull_DoesNotChange()
        {
            var request = new SchoolsExperienceSignUp() { SecondaryPreferredTeachingSubjectId = null };

            request.Candidate.ChangedPropertyNames.Should().NotContain("SecondaryPreferredTeachingSubjectId");
        }
    }
}
