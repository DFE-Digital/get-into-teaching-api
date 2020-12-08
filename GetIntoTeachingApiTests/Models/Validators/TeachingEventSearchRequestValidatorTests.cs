using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
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
            var mockPickListItem = new TypeEntity { Id = "123" };

            _mockStore
                .Setup(mock => mock.GetPickListItems("msevtmgt_event", "dfe_event_type"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var request = new TeachingEventSearchRequest()
            {
                Postcode = "KY11 9HF",
                Radius = 10,
                TypeId = int.Parse(mockPickListItem.Id),
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
            _validator.ShouldHaveValidationErrorFor(request => request.TypeId, 123);
        }

        [Fact]
        public void Validate_TypeIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(request => request.TypeId, null as int?);
        }

        [Fact]
        public void Validate_PostcodeIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Postcode, "");
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
            if (hasError)
            {
                _validator.ShouldHaveValidationErrorFor(request => request.Postcode, postcode);
            }
            else
            {
                _validator.ShouldNotHaveValidationErrorFor(request => request.Postcode, postcode);
            }
        }

        [Fact]
        public void Validate_PostcodeIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(request => request.Postcode, null as string);
        }

        [Fact]
        public void Validate_RadiusIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(request => request.Radius, null as int?);
        }

        [Fact]
        public void Validate_RadiusIsNegative_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Radius, -1);
        }
    }
}
