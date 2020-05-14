using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class ExistingCandidateRequestValidatorTests
    {
        private readonly ExistingCandidateRequestValidator _validator;

        public ExistingCandidateRequestValidatorTests()
        {
            _validator = new ExistingCandidateRequestValidator();
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "first", DateOfBirth = DateTime.Now.AddDays(-18) };

            var result = _validator.TestValidate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmailAddressIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Email, "");
        }

        [Fact]
        public void Validate_EmailAddressIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Email, "invalid-email@");
        }

        [Fact]
        public void Validate_EmailAddressTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Email, $"{new string('a', 50)}@{new string('a', 50)}.com");
        }

        [Fact]
        public void Validate_EmailAddressPresent_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(request => request.Email, "valid@email.com");
        }

        [Fact]
        public void Validate_DateOfBirthInFuture_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.DateOfBirth, DateTime.Now.AddDays(1));
        }

        [Fact]
        public void Validate_FirstNameTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.FirstName, new string('a', 257));
        }

        [Fact]
        public void Validate_LastNameTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.LastName, new string('a', 257));
        }

        [Fact]
        public void Validate_SpecifyNoAdditionalRequiredAttributes_HasError()
        {
            var request = new ExistingCandidateRequest();

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request).WithErrorMessage("You must specify values for 2 additional attributes (from birthdate, firstname and lastname).");
        }

        [Fact]
        public void Validate_SpecifyOneAdditionalRequiredAttributes_HasError()
        {
            var request = new ExistingCandidateRequest { FirstName = "first" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request).WithErrorMessage("You must specify values for 2 additional attributes (from birthdate, firstname and lastname).");
        }

        [Fact]
        public void Validate_SpecifyTwoAdditionalRequiredAttributes_HasNoError()
        {
            var request = new ExistingCandidateRequest { FirstName = "first", LastName = "last" };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request);
        }

        [Fact]
        public void Validate_SpecifyThreeAdditionalRequiredAttributes_HasNoError()
        {
            var request = new ExistingCandidateRequest { FirstName = "first", LastName = "last", DateOfBirth = DateTime.Now.AddDays(-18) };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request);
        }
    }
}
