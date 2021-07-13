using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.SchoolsExperience;
using GetIntoTeachingApi.Models.Validators.SchoolsExperience;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators.SchoolsExperience
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
        public void Validate_FirstNameIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.FirstName, null as string);
        }

        [Fact]
        public void Validate_LastNameIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.LastName, null as string);
        }

        [Fact]
        public void Validate_EmailIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Email, null as string);
        }

        [Fact]
        public void Validate_DateOfBirthIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.DateOfBirth, null as DateTime?);
        }

        [Fact]
        public void Validate_AddressLine1IsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AddressLine1, null as string);
        }

        [Fact]
        public void Validate_AddressCityIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AddressCity, null as string);
        }

        [Fact]
        public void Validate_AddressStateOrProvinceIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AddressStateOrProvince, null as string);
        }

        [Fact]
        public void Validate_AddressPostcodeIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AddressPostcode, null as string);
        }

        [Fact]
        public void Validate_AddressTelephoneIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AddressTelephone, null as string);
        }

        [Fact]
        public void Validate_TelephoneIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Telephone, null as string);
        }

        [Fact]
        public void Validate_SecondaryTelephoneIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.SecondaryTelephone, null as string);
        }

        [Fact]
        public void Validate_HasDbsCertificateIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.HasDbsCertificate, null as bool?);
        }

        [Fact]
        public void Validate_AcceptedPolicyIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AcceptedPolicyId, null as Guid?);
        }

        [Fact]
        public void Validate_PreferredTeachingSubjectIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.PreferredTeachingSubjectId, null as Guid?);
        }

        [Fact]
        public void Validate_SecondaryPreferredTeachingSubjectIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(request => request.SecondaryPreferredTeachingSubjectId, null as Guid?);
        }
    }
}
