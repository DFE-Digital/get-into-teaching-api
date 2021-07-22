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
        public void Validate_ApiKeyPrefixInvalidFormat_HasErrors(string value)
        {
            _validator.ShouldHaveValidationErrorFor(client => client.ApiKeyPrefix, value);
        }

        [Theory]
        [InlineData("K")]
        [InlineData("KE")]
        [InlineData("KEY")]
        [InlineData("KE_Y")]
        [InlineData("KEY_KEY_KEY")]
        public void Validate_ApiKeyPrefixValidFormat_HasNoErrors(string value)
        {
            _validator.ShouldNotHaveValidationErrorFor(client => client.ApiKeyPrefix, value);
        }

        [Theory]
        [InlineData("role")]
        [InlineData("roleAdmin")]
        [InlineData("Admin Role")]
        [InlineData("Admin_Role")]
        [InlineData("admin_role")]
        public void Validate_RoleInvalidFormat_HasErrors(string value)
        {
            _validator.ShouldHaveValidationErrorFor(client => client.Role, value);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("AdminRole")]
        public void Validate_RoleValidFormat_HasNoErrors(string value)
        {
            _validator.ShouldNotHaveValidationErrorFor(client => client.Role, value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Validate_InvalidName_HasError(string value)
        {
            _validator.ShouldHaveValidationErrorFor(client => client.Name, value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Validate_InvalidDescription_HasError(string value)
        {
            _validator.ShouldHaveValidationErrorFor(client => client.Description, value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Validate_InvalidApiKeyPrefixHasError(string value)
        {
            _validator.ShouldHaveValidationErrorFor(client => client.ApiKeyPrefix, value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Validate_InvalidRole_HasError(string value)
        {
            _validator.ShouldHaveValidationErrorFor(client => client.Role, value);
        }
    }
}