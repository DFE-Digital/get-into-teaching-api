using System;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class PhoneCallValidatorTests
    {
        private readonly PhoneCallValidator _validator;

        public PhoneCallValidatorTests()
        {
            _validator = new PhoneCallValidator();
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var phoneCall = new PhoneCall()
            {
                ScheduledAt = DateTime.Now.AddDays(2),
            };

            var result = _validator.TestValidate(phoneCall);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ScheduledAtInPast_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(phoneCall => phoneCall.ScheduledAt, DateTime.Now.AddDays(-1));
        }
    }
}
