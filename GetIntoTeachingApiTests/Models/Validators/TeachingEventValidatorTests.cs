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
    public class TeachingEventValidatorTests
    {
        private readonly TeachingEventValidator _validator;
        private readonly Mock<ICrmService> _mockCrm;

        public TeachingEventValidatorTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _validator = new TeachingEventValidator(_mockCrm.Object, new DateTimeProvider());
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var client = new TeachingEvent()
            {
                ReadableId = "Test",
                Name = "Test name",
                Summary = "Test summary",
                Description = "Test description",
                ProviderContactEmail = "test@test.com",
                StartAt = DateTime.UtcNow.AddDays(1),
                EndAt = DateTime.UtcNow.AddDays(1),
            };

            var result = _validator.Validate(client);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ReadableIdIsNullOrEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(teachingEvent => teachingEvent.ReadableId, "");
            _validator.ShouldHaveValidationErrorFor(teachingEvent => teachingEvent.ReadableId, null as string);
        }

        [Fact]
        public void Validate_IdIsNullAndReadableIdIsNotUnique_HasError()
        {
            const string nonUniqueReadableId = "not_unique";
            var existingTeachingEvent = new TeachingEvent
            {
                ReadableId = nonUniqueReadableId,
            };

            _mockCrm
                .Setup(mock => mock.GetTeachingEvent(nonUniqueReadableId))
                .Returns(existingTeachingEvent);

            var teachingEvent = new TeachingEvent
            {
                Id = null,
                ReadableId = nonUniqueReadableId,
            };

            var result = _validator.TestValidate(teachingEvent);

            result
                .ShouldHaveValidationErrorFor(teachingEvent => teachingEvent.ReadableId)
                .WithErrorMessage("Must be unique");
        }

        [Fact]
        public void Validate_NameIsNullOrEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(teachingEvent => teachingEvent.Name, "");
            _validator.ShouldHaveValidationErrorFor(teachingEvent => teachingEvent.Name, null as string);
        }

        [Fact]
        public void Validate_ProviderContactEmailIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(teachingEvent => teachingEvent.ProviderContactEmail, null as string);
        }

        [Fact]
        public void Validate_ProviderContactEmailIsPresentAndInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(teachingEvent => teachingEvent.ProviderContactEmail, "Invalid email");
        }

        [Fact]
        public void Validate_StartAtIsEarlierThanNow_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(teachingEvent => teachingEvent.StartAt, DateTime.UtcNow.AddDays(-1));
        }

        [Fact]
        public void Validate_EndAtIsEarlierThanStartAt_HasError()
        {
            var invalidTeachingEvent = new TeachingEvent
            {
                StartAt = DateTime.UtcNow.AddDays(2),
                EndAt = DateTime.UtcNow.AddDays(1)
            };
            _validator.ShouldHaveValidationErrorFor(teachingEvent => teachingEvent.EndAt, invalidTeachingEvent);
        }
    }
}

