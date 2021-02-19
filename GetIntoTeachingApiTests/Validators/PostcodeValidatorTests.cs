using GetIntoTeachingApi.Validators;
using Xunit;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using System.Linq;
using FluentAssertions;

namespace GetIntoTeachingApiTests.Validators
{
    public class PostcodeValidatorTests
    {
        private readonly PostcodeValidator _validator;

        public PostcodeValidatorTests()
        {
            _validator = new PostcodeValidator();
        }

        [Theory]
        [InlineData("KY119YU", true)]
        [InlineData("KY11 9YU", true)]
        [InlineData("CA48LE", true)]
        [InlineData("CA4 8LE", true)]
        [InlineData("ky119yu", true)]
        [InlineData("KY999 9YU", false)]
        [InlineData("AZ1VS1", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsValid_WithPostcode_ReturnsCorectly(string postcode, bool expectedValid)
        {
            var selector = ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory();
            var validationContext = new ValidationContext(postcode, new PropertyChain(), selector);
            var propertyValidatorContext = new PropertyValidatorContext(validationContext, PropertyRule.Create<string, string>(t => t), "Prop");

            var errors = _validator.Validate(propertyValidatorContext);

            if (expectedValid)
            {
                errors.Should().BeEmpty();
            }
            else
            {
                errors.Should().NotBeEmpty();
                errors.First().ErrorMessage.Should().Equals("Prop must be a valid postcode.");
            }
        }
    }
}
