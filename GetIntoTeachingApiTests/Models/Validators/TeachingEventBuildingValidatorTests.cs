using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models.Crm;
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
        public void Validate_NameAndPostcodeIsNullOrEmpty_HasError()
        {
            var building = new TeachingEventBuilding() { Venue = "", AddressPostcode = "" };
            var result = _validator.TestValidate(building);

            result.ShouldHaveValidationErrorFor(building => building.Venue);
            result.ShouldHaveValidationErrorFor(building => building.AddressPostcode);

            building.Venue = null;
            building.AddressPostcode = null;
            result = _validator.TestValidate(building);

            result.ShouldHaveValidationErrorFor(building => building.Venue);
            result.ShouldHaveValidationErrorFor(building => building.AddressPostcode);
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
            var building = new TeachingEventBuilding() { AddressPostcode = postcode };
            var result = _validator.TestValidate(building);

            if (hasError)
            {
                result.ShouldHaveValidationErrorFor(building => building.AddressPostcode);
            }
            else
            {
                result.ShouldNotHaveValidationErrorFor(building => building.AddressPostcode);
            }
        }
    }
}
