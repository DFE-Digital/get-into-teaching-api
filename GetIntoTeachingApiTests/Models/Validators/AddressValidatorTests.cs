using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class AddressValidatorTests
    {
        private readonly AddressValidator _validator;

        public AddressValidatorTests()
        {
            _validator = new AddressValidator();
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var address = new Address
            {
                Line1 = "line1", 
                Line2 = "line2", 
                Line3 = "line3",
                City = "city", 
                State = "state", 
                Postcode = "postcode"
            };

            var result = _validator.TestValidate(address);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Line1IsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.Line1, "");
        }

        [Fact]
        public void Validate_Line1IsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.Line1, new string('a', 1025));
        }

        [Fact]
        public void Validate_Line2IsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.Line2, new string('a', 1025));
        }

        [Fact]
        public void Validate_Line3IsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.Line3, new string('a', 1025));
        }

        [Fact]
        public void Validate_CityIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.City, "");
        }

        [Fact]
        public void Validate_CityIsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.City, new string('a', 129));
        }

        [Fact]
        public void Validate_StateIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.State, "");
        }

        [Fact]
        public void Validate_StateIsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.State, new string('a', 129));
        }

        [Fact]
        public void Validate_PostcodeIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.Postcode, "");
        }

        [Fact]
        public void Validate_PostcodeIsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.Postcode, new string('a', 41));
        }
    }
}
