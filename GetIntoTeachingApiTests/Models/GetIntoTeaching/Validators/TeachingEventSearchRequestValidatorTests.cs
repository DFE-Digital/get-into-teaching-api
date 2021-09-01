using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Models.GetIntoTeaching.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching.Validators
{
    public class TeachingEventSearchRequestValidatorTests
    {
        private readonly TeachingEventSearchRequestValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public TeachingEventSearchRequestValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new TeachingEventSearchRequestValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new PickListItem { Id = 123 };

            _mockStore
                .Setup(mock => mock.GetPickListItems("msevtmgt_event", "dfe_event_type"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var request = new TeachingEventSearchRequest()
            {
                Postcode = "KY11 9HF",
                Radius = 10,
                TypeIds = new int[] { mockPickListItem.Id },
                StartAfter = DateTime.UtcNow.AddDays(-1),
                StartBefore = DateTime.UtcNow.AddDays(1)
            };

            var result = _validator.TestValidate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_StartAfterLaterThanStartBefore_HasError()
        {
            var request = new TeachingEventSearchRequest()
            {
                StartAfter = DateTime.UtcNow.AddDays(1),
                StartBefore = DateTime.UtcNow
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request).WithErrorMessage("Start after must be earlier than start before.");
        }

        [Fact]
        public void Validate_StartAfterAndNoStartBefore_HasNoError()
        {
            var request = new TeachingEventSearchRequest()
            {
                StartAfter = DateTime.UtcNow.AddDays(1),
                StartBefore = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_StartBeforeAndNoStartAfter_HasNoError()
        {
            var request = new TeachingEventSearchRequest()
            {
                StartBefore = DateTime.UtcNow.AddDays(1),
                StartAfter = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_RadiusIsNotNullAndPostcodeIsNull_HasError()
        {
            var request = new TeachingEventSearchRequest()
            {
                Radius = 10,
                Postcode = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request.Postcode);
        }

        [Fact]
        public void Validate_TypeIdIsInvalid_HasError()
        {
            var result = _validator.TestValidate(new TeachingEventSearchRequest() { TypeIds = new int[] { 123 } });

            result.ShouldHaveValidationErrorFor(r => r.TypeIds);
        }

        [Fact]
        public void Validate_PostcodeIsEmpty_HasError()
        {
            var result = _validator.TestValidate(new TeachingEventSearchRequest() { Postcode = "" });

            result.ShouldHaveValidationErrorFor(r => r.Postcode);
        }

        [Theory]
        [InlineData("KY119YU", false)]
        [InlineData("KY11 9YU", false)]
        [InlineData("CA48LE", false)]
        [InlineData("CA4 8LE", false)]
        [InlineData("ky119yu", false)]
        [InlineData("KY999 9YU", true)]
        [InlineData("AZ1VS1", true)]
        [InlineData("KY11", false)]
        [InlineData("KY999", true)]
        [InlineData("TE57 ING", true)]
        public void Validate_PostcodeFormat_ValidatesCorrectly(string postcode, bool hasError)
        {
            var result = _validator.TestValidate(new TeachingEventSearchRequest() { Postcode = postcode });

            if (hasError)
            {
                result.ShouldHaveValidationErrorFor(r => r.Postcode);
            }
            else
            {
                result.ShouldNotHaveValidationErrorFor(r => r.Postcode);
            }
        }

        [Fact]
        public void Validate_RadiusIsNegative_HasError()
        {
            var result = _validator.TestValidate(new TeachingEventSearchRequest() { Radius = -1 });

            result.ShouldHaveValidationErrorFor(r => r.Radius);
        }
    }
}
