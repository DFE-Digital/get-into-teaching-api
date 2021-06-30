using GetIntoTeachingApi.Validators;
using Xunit;
using FluentValidation;
using FluentAssertions;

namespace GetIntoTeachingApiTests.Validators
{
    public class PostcodeValidatorTests
    {
        private readonly PostcodeValidator<object> _validator;

        public PostcodeValidatorTests()
        {
            _validator = new PostcodeValidator<object>();
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
            var context = new ValidationContext<object>(this);

            var valid =_validator.IsValid(context, postcode);

            valid.Should().Be(expectedValid);
        }
    }
}
