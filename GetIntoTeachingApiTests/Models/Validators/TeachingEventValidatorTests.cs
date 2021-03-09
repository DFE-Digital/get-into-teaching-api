using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class TeachingEventValidatorTests
    {
        private readonly TeachingEventValidator _validator;

        public TeachingEventValidatorTests()
        {
            _validator = new TeachingEventValidator();
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var client = new TeachingEvent()
            {
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
        public void Validate_NameIsNull_HasError()
        {
            var teachingEvent = new TeachingEvent { Name = null };
            var validationResult = _validator.TestValidate(teachingEvent);
            validationResult.ShouldHaveValidationErrorFor(request => request.Name);
        }

        [Fact]
        public void Validate_SummaryIsNull_HasError()
        {
            var teachingEvent = new TeachingEvent { Summary = null };
            var validationResult = _validator.TestValidate(teachingEvent);
            validationResult.ShouldHaveValidationErrorFor(request => request.Summary);
        }

        [Fact]
        public void Validate_DescriptionIsNull_HasError()
        {
            var teachingEvent = new TeachingEvent { Description = null };
            var validationResult = _validator.TestValidate(teachingEvent);
            validationResult.ShouldHaveValidationErrorFor(request => request.Description);
        }

        [Fact]
        public void Validate_ProviderContactEmailIsPresentAndInvalid_HasError()
        {
            var teachingEvent = new TeachingEvent { ProviderContactEmail = "invalid email" };
            var validationResult = _validator.TestValidate(teachingEvent);
            validationResult.ShouldHaveValidationErrorFor(request => request.ProviderContactEmail);
        }

        [Fact]
        public void Validate_StartAtIsEarlierThanNow_HasError()
        {
            var teachingEvent = new TeachingEvent { StartAt = DateTime.UtcNow.AddDays(-1) };
            var validationResult = _validator.TestValidate(teachingEvent);
            validationResult.ShouldHaveValidationErrorFor(request => request.StartAt);
        }

        [Fact]
        public void Validate_EndAtIsEarlierThanStartAt_HasError()
        {
            var teachingEvent = new TeachingEvent
            {
                StartAt = DateTime.UtcNow.AddDays(2),
                EndAt = DateTime.UtcNow.AddDays(1)
            };
            var validationResult = _validator.TestValidate(teachingEvent);
            validationResult.ShouldHaveValidationErrorFor(request => request.EndAt);
        }
    }
}

