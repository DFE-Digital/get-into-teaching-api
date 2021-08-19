using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class ClientValidatorTests
    {
        private readonly ClientValidator _validator;

        public ClientValidatorTests()
        {
            _validator = new ClientValidator();
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var client = new Client()
            {
                Name = "Get an Adviser",
                Description = "A Rails application that enables candidates to sign up for a teacher training adviser.",
                Role = "GetAnAdvisor",
                ApiKeyPrefix = "GET_AN_ADVISOR",
            };

            var result = _validator.Validate(client);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("key")]
        [InlineData("ke y")]
        [InlineData("KE Y")]
        [InlineData("KE _Y")]
        [InlineData("_KEY")]
        [InlineData("KEY_")]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Validate_ApiKeyPrefixInvalidFormat_HasErrors(string value)
        {
            var request = new Client() { ApiKeyPrefix = value };
            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(c => c.ApiKeyPrefix);
        }

        [Theory]
        [InlineData("K")]
        [InlineData("KE")]
        [InlineData("KEY")]
        [InlineData("KE_Y")]
        [InlineData("KEY_KEY_KEY")]
        public void Validate_ApiKeyPrefixValidFormat_HasNoErrors(string value)
        {
            var request = new Client() { ApiKeyPrefix = value };
            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(c => c.ApiKeyPrefix);
        }

        [Theory]
        [InlineData("role")]
        [InlineData("roleAdmin")]
        [InlineData("Admin Role")]
        [InlineData("Admin_Role")]
        [InlineData("admin_role")]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Validate_RoleInvalidFormat_HasErrors(string value)
        {
            var request = new Client() { Role = value };
            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(c => c.Role);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("AdminRole")]
        public void Validate_RoleValidFormat_HasNoErrors(string value)
        {
            var request = new Client() { Role = value };
            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(c => c.Role);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Validate_InvalidName_HasError(string value)
        {
            var request = new Client() { Name = value };
            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Validate_InvalidDescription_HasError(string value)
        {
            var request = new Client() { Description = value };
            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(c => c.Description);
        }
    }
}