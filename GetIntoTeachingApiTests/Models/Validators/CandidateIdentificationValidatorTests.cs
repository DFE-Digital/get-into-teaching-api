using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models.Validators;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class CandidateIdentificationValidatorTests
    {
        private readonly CandidateIdentificationValidator _validator;

        public CandidateIdentificationValidatorTests()
        {
            _validator = new CandidateIdentificationValidator();
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var member = new GetIntoTeachingApi.Models.CandidateIdentification { Email = "email@address.com", FirstName = "first", LastName = "last" };

            var result = _validator.TestValidate(member);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmailAddressIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(member => member.Email, null as string);
        }

        [Fact]
        public void Validate_EmailAddressIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(member => member.Email, "");
        }

        [Fact]
        public void Validate_EmailAddressIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(member => member.Email, "invalid-email@");
        }

        [Fact]
        public void Validate_EmailAddressPresent_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(member => member.Email, "valid@email.com");
        }

        [Fact]
        public void Validate_FirstNameIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(member => member.FirstName, "");
        }

        [Fact]
        public void Validate_LastNameIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(member => member.LastName, "");
        }
    }
}
