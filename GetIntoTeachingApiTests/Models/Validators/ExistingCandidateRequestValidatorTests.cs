using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class ExistingCandidateRequestValidatorTests
    {
        private readonly ExistingCandidateRequestValidator _validator;

        public ExistingCandidateRequestValidatorTests()
        {
            _validator = new ExistingCandidateRequestValidator(new DateTimeProvider());
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "first", DateOfBirth = DateTime.UtcNow.AddDays(-18) };

            var result = _validator.TestValidate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmailAddressIsEmpty_HasError()
        {
            var result = _validator.TestValidate(new ExistingCandidateRequest() { Email = "" });

            result.ShouldHaveValidationErrorFor(r => r.Email);
        }


        [Fact]
        public void Validate_EmailAddressIsInvalid_HasError()
        {
            var result = _validator.TestValidate(new ExistingCandidateRequest() { Email = "invalid-email@" });

            result.ShouldHaveValidationErrorFor(r => r.Email);
        }

        [Fact]
        public void Validate_EmailAddressTooLong_HasError()
        {
            var result = _validator.TestValidate(new ExistingCandidateRequest() { Email = $"{new string('a', 50)}@{new string('a', 50)}.com" });

            result.ShouldHaveValidationErrorFor(r => r.Email);
        }

        [Fact]
        public void Validate_EmailAddressPresent_HasNoError()
        {
            var result = _validator.TestValidate(new ExistingCandidateRequest() { Email = "valid@email.com" });

            result.ShouldNotHaveValidationErrorFor(r => r.Email);
        }

        [Fact]
        public void Validate_DateOfBirthInFuture_HasError()
        {
            var result = _validator.TestValidate(new ExistingCandidateRequest() { DateOfBirth = DateTime.UtcNow.AddDays(1) });

            result.ShouldHaveValidationErrorFor(r => r.DateOfBirth);
        }

        [Fact]
        public void Validate_NameIsTooLong_HasError()
        {
            var tooLongName = new string('a', 257);
            var request = new ExistingCandidateRequest() { FirstName = tooLongName, LastName = tooLongName };
            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(r => r.FirstName);
            result.ShouldHaveValidationErrorFor(r => r.LastName);
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
            var request = new ExistingCandidateRequest { FirstName = "first", LastName = "last", DateOfBirth = DateTime.UtcNow.AddDays(-18) };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request);
        }
    }
}
