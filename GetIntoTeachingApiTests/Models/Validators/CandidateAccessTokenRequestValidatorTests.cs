using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class CandidateAccessTokenRequestValidatorTests
    {
        private readonly CandidateAccessTokenRequestValidator _validator;

        public CandidateAccessTokenRequestValidatorTests()
        {
            _validator = new CandidateAccessTokenRequestValidator();
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var request = new CandidateAccessTokenRequest { Email = "email@address.com", FirstName = "first", DateOfBirth = DateTime.Now.AddDays(-18) };

            var result = _validator.TestValidate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmailAddressIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Email, null as string);
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
        public void Validate_DateOfBirthInPast_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(request => request.DateOfBirth, DateTime.Now.AddDays(-1));
        }

        [Fact]
        public void Validate_SpecifyNoAdditionalRequiredAttributes_HasError()
        {
            var request = new CandidateAccessTokenRequest();

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request).WithErrorMessage("You must specify values for 2 additional attributes (from birthdate, firstname and lastname).");
        }

        [Fact]
        public void Validate_SpecifyOneAdditionalRequiredAttributes_HasError()
        {
            var request = new CandidateAccessTokenRequest { FirstName = "first" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request).WithErrorMessage("You must specify values for 2 additional attributes (from birthdate, firstname and lastname).");
        }

        [Fact]
        public void Validate_SpecifyTwoAdditionalRequiredAttributes_HasNoError()
        {
            var request = new CandidateAccessTokenRequest { FirstName = "first", LastName = "last" };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request);
        }

        [Fact]
        public void Validate_SpecifyThreeAdditionalRequiredAttributes_HasNoError()
        {
            var request = new CandidateAccessTokenRequest { FirstName = "first", LastName = "last", DateOfBirth = DateTime.Now.AddDays(-18) };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request);
        }
    }
}
