using System;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.Validators;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.Validators
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
        public void Validate_WhenRequiredAttributesAreNullOrEmpty_HasErrors()
        {
            var teachingEvent = new TeachingEvent()
            {
                ReadableId = "",
                Name = "",
                ProviderContactEmail = "",
            };
            var result = _validator.TestValidate(teachingEvent);

            result.ShouldHaveValidationErrorFor(e => e.ReadableId);
            result.ShouldHaveValidationErrorFor(e => e.Name);
            result.ShouldHaveValidationErrorFor(e => e.ProviderContactEmail);
        }


        [Fact]
        public void Validate_ProviderContactEmailIsPresentAndInvalid_HasError()
        {
            var result = _validator.TestValidate(new TeachingEvent() { ProviderContactEmail = "invalid-email@" });

            result.ShouldHaveValidationErrorFor(c => c.ProviderContactEmail);
        }

        [Fact]
        public void Validate_EndAtIsEarlierThanStartAt_HasError()
        {
            var invalidTeachingEvent = new TeachingEvent
            {
                StartAt = DateTime.UtcNow.AddDays(2),
                EndAt = DateTime.UtcNow.AddDays(1)
            };
            var result = _validator.TestValidate(invalidTeachingEvent);

            result.ShouldHaveValidationErrorFor(c => c.EndAt);
        }
    }
}

