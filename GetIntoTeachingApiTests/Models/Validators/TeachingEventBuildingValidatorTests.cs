using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models.Validators;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class TeachingEventBuildingValidatorTests
    {
        private readonly TeachingEventBuildingValidator _validator;

        public TeachingEventBuildingValidatorTests()
        {
            _validator = new TeachingEventBuildingValidator();
        }

        [Fact]
        public void Validate_NameIsNullOrEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(building => building.Venue, "");
            _validator.ShouldHaveValidationErrorFor(building => building.Venue, null as string);
        }

        [Fact]
        public void Validate_AddressPostcodeIsNullOrEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(building => building.AddressPostcode, "");
            _validator.ShouldHaveValidationErrorFor(building => building.AddressPostcode, null as string);
        }

        [Theory]
        [InlineData("KY119YU", false)]
        [InlineData("KY11 9YU", false)]
        [InlineData("CA48LE", false)]
        [InlineData("CA4 8LE", false)]
        [InlineData("ky119yu", false)]
        [InlineData("KY999 9YU", true)]
        [InlineData("AZ1VS1", true)]
        public void Validate_AddressPostcodeFormat_ValidatesCorrectly(string postcode, bool hasError)
        {
            if (hasError)
            {
                _validator.ShouldHaveValidationErrorFor(building => building.AddressPostcode, postcode);
            }
            else
            {
                _validator.ShouldNotHaveValidationErrorFor(building => building.AddressPostcode, postcode);
            }
        }
    }
}
