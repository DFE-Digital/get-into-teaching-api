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
            _validator = new ExistingCandidateRequestValidator();
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "First" };

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
        public void Validate_FirstNameIsEmpty_HasError()
        {
            var result = _validator.TestValidate(new ExistingCandidateRequest() { FirstName = "" });

            result.ShouldHaveValidationErrorFor(r => r.FirstName);
        }
    }
}
