using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.SchoolsExperience;
using GetIntoTeachingApi.Models.SchoolsExperience.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.SchoolsExperience.Validators
{
    public class SchoolsExperienceSignUpValidatorTests
    {
        private readonly SchoolsExperienceSignUpValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public SchoolsExperienceSignUpValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new SchoolsExperienceSignUpValidator(_mockStore.Object, new DateTimeProvider());
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new PickListItem { Id = 123 };
            var mockLookupItem = new LookupItem { Id = Guid.NewGuid() };
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };

            var request = new SchoolsExperienceSignUp()
            {
                CandidateId = null,
                PreferredTeachingSubjectId = mockLookupItem.Id,
                SecondaryPreferredTeachingSubjectId = mockLookupItem.Id,
                AcceptedPolicyId = (Guid)mockPrivacyPolicy.Id,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(2000, 1, 1),
                AddressLine1 = "Address 1",
                AddressCity = "City",
                AddressStateOrProvince = "County",
                AddressPostcode = "KY11 9YU",
                AddressTelephone = "123456789",
                Telephone = "123456789",
                SecondaryTelephone = "123456789",
                HasDbsCertificate = false,
            };

            var result = _validator.TestValidate(request);

            // Ensure no validation errors on root object (we expect errors on the Candidate
            // properties as we can't mock them).
            var propertiesWithErrors = result.Errors.Select(e => e.PropertyName);
            propertiesWithErrors.All(p => p.StartsWith("Candidate.")).Should().BeTrue();
        }

        [Fact]
        public void Validate_CandidateIsInvalid_HasError()
        {
            var request = new SchoolsExperienceSignUp
            {
                Telephone = "123",
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("Candidate.Telephone");
        }

        [Fact]
        public void Validate_RequiredFieldsWhenNull_HasError()
        {
            var signUp = new SchoolsExperienceSignUp()
            {
                FirstName = null,
                LastName = null,
                Email = null,
                DateOfBirth = null,
                AddressLine1 = null,
                AddressCity = null,
                AddressStateOrProvince = null,
                AddressPostcode = null,
                AddressTelephone = null,
                Telephone = null,
                SecondaryTelephone = null,
                HasDbsCertificate = null,
                AcceptedPolicyId = null,
                PreferredTeachingSubjectId = null,

            };
            var result = _validator.TestValidate(signUp);

            result.ShouldHaveValidationErrorFor(s => s.FirstName);
            result.ShouldHaveValidationErrorFor(s => s.LastName);
            result.ShouldHaveValidationErrorFor(s => s.Email);
            result.ShouldHaveValidationErrorFor(s => s.DateOfBirth);
            result.ShouldHaveValidationErrorFor(s => s.AddressLine1);
            result.ShouldHaveValidationErrorFor(s => s.AddressCity);
            result.ShouldHaveValidationErrorFor(s => s.AddressStateOrProvince);
            result.ShouldHaveValidationErrorFor(s => s.AddressPostcode);
            result.ShouldHaveValidationErrorFor(s => s.AddressTelephone);
            result.ShouldHaveValidationErrorFor(s => s.Telephone);
            result.ShouldHaveValidationErrorFor(s => s.SecondaryTelephone);
            result.ShouldHaveValidationErrorFor(s => s.HasDbsCertificate);
            result.ShouldHaveValidationErrorFor(s => s.AcceptedPolicyId);
            result.ShouldHaveValidationErrorFor(s => s.PreferredTeachingSubjectId);
        }
    }
}
