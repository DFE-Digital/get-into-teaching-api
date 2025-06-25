using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Models.GetIntoTeaching.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching.Validators
{
    public class GetIntoTeachingCallbackValidatorTests
    {
        private readonly GetIntoTeachingCallbackValidator _validator;
        private readonly Mock<IStore> _mockStore;
        private readonly GetIntoTeachingCallback _callback;

        public GetIntoTeachingCallbackValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new GetIntoTeachingCallbackValidator(_mockStore.Object, new DateTimeProvider());
            _callback = new GetIntoTeachingCallback();
        }
        
        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            // var mockPickListItem = new PickListItem { Id = 123 };
            // var mockSubject = new TeachingSubject { Id = Guid.NewGuid() };
            // var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };

            var request = new GetIntoTeachingCallback()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "email@address.com",
                AcceptedPolicyId = Guid.NewGuid(),
                AddressTelephone = "123456789",
                PhoneCallScheduledAt = DateTime.UtcNow.AddDays(1),
                TalkingPoints = "Talking points",
                CreationChannelSourceId = 222750003,
                CreationChannelServiceId = 222750002,
                CreationChannelActivityId = 222750001,
            };

            var result = _validator.TestValidate(request);
            // Ensure no validation errors on root object (we expect errors on the Candidate
            // properties as we can't mock them).
            var propertiesWithErrors = result.Errors.Select(e => e.PropertyName);
            propertiesWithErrors.All(p => p.StartsWith("Candidate.")).Should().BeTrue();
        }

        [Fact]
        public void Validate_RequiredFieldsWhenNull_HasError()
        {
            var callback = new GetIntoTeachingCallback()
            {
                FirstName = null,
                LastName = null,
                Email = null,
                AcceptedPolicyId = null,
                AddressTelephone = null,
                PhoneCallScheduledAt = null,
                TalkingPoints = null,
            };
            var result = _validator.TestValidate(callback);

            result.ShouldHaveValidationErrorFor(c => c.FirstName);
            result.ShouldHaveValidationErrorFor(c => c.LastName);
            result.ShouldHaveValidationErrorFor(c => c.Email);
            result.ShouldHaveValidationErrorFor(c => c.AddressTelephone);
            result.ShouldHaveValidationErrorFor(c => c.PhoneCallScheduledAt);
            result.ShouldHaveValidationErrorFor(c => c.AcceptedPolicyId);
            result.ShouldHaveValidationErrorFor(c => c.TalkingPoints);
        }

        [Fact]
        public void Validate_CandidateIsInvalid_HasError()
        {
            var request = new GetIntoTeachingCallback
            {
                AddressTelephone = "123",
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("Candidate.AddressTelephone");
        }

        [Fact]
        public void Validate_WhenPhoneCallScheduledAtIsInPast_HasError()
        {
            _callback.PhoneCallScheduledAt = DateTime.UtcNow.AddDays(-1);

            var result = _validator.TestValidate(_callback);

            result.ShouldHaveValidationErrorFor(request => request.PhoneCallScheduledAt)
                .WithErrorMessage("Can only be scheduled for future dates.");

            _callback.PhoneCallScheduledAt = DateTime.UtcNow.AddDays(1);

            result = _validator.TestValidate(_callback);

            result.ShouldNotHaveValidationErrorFor(request => request.PhoneCallScheduledAt);
        }
    }
}
