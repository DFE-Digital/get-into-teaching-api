using System;
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
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<ILocationService> _mockLocationService;

        public TeachingEventSearchRequestValidatorTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _mockLocationService = new Mock<ILocationService>();
            _validator = new TeachingEventSearchRequestValidator(_mockCrm.Object, _mockLocationService.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockType = new TypeEntity { Id = 123 };

            _mockCrm
                .Setup(mock => mock.GetPickListItems("msevtmgt_event", "dfe_event_type"))
                .Returns(new[] { mockType });

            _mockLocationService.Setup(mock => mock.IsValid("KY11 9HF")).Returns(true);

            var request = new TeachingEventSearchRequest()
            {
                Postcode = "KY11 9HF",
                Radius = 10,
                TypeId = mockType.Id,
                StartAfter = DateTime.Now.AddDays(-1),
                StartBefore = DateTime.Now.AddDays(1)
            };

            var result = _validator.TestValidate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_StartAfterLaterThanStartBefore_HasError()
        {
            var request = new TeachingEventSearchRequest()
            {
                StartAfter = DateTime.Now.AddDays(1),
                StartBefore = DateTime.Now
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request).WithErrorMessage("Start after must be earlier than start before.");
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
