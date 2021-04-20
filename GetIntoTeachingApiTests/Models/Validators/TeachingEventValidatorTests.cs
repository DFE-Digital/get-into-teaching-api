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
        private readonly Mock<IStore> _mockStore;

        public TeachingEventValidatorTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _mockStore = new Mock<IStore>();
            _validator = new TeachingEventValidator(_mockCrm.Object, _mockStore.Object);
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
            var existingTeachingEvent = new TeachingEvent
            {
                ReadableId = "not_unique",
            };

            _mockCrm
                .Setup(mock => mock.GetTeachingEvent("not_unique"))
                .Returns(existingTeachingEvent);

            var newTeachingEvent = new TeachingEvent
            {
                ReadableId = "not_unique",
            };

            var result = _validator.TestValidate(newTeachingEvent);

            result
                .ShouldHaveValidationErrorFor(teachingEvent => teachingEvent.ReadableId)
                .WithErrorMessage("Must be unique");
        }

        [Fact]
        public void Validate_IdIsNotNullAndReadableIdHasChanged_HasError()
        {
            var existingTeachingEvent1 = new TeachingEvent
            {
                Id = Guid.NewGuid(),
                ReadableId = "unique",
            };

            var existingTeachingEvent2 = new TeachingEvent
            {
                ReadableId = "not_unique",
            };

            _mockStore
                .Setup(mock => mock.TeachingEventExistsWithReadableId(
                    (Guid)existingTeachingEvent1.Id, existingTeachingEvent2.ReadableId))
                .Returns(false);

            _mockCrm
                .Setup(mock => mock.GetTeachingEvent("not_unique"))
                .Returns(existingTeachingEvent2);

            var teachingEvent = new TeachingEvent
            {
                Id = existingTeachingEvent1.Id,
                ReadableId = "not_unique",
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

