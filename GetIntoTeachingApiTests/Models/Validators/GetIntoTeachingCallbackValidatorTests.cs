﻿using System;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
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
        public void Validate_WhenRequiredAttributesAreNull_HasErrors()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.FirstName, null as string);
            _validator.ShouldHaveValidationErrorFor(request => request.LastName, null as string);
            _validator.ShouldHaveValidationErrorFor(request => request.Email, null as string);
            _validator.ShouldHaveValidationErrorFor(request => request.AddressTelephone, null as string);
            _validator.ShouldHaveValidationErrorFor(request => request.PhoneCallScheduledAt, null as DateTime?);
            _validator.ShouldHaveValidationErrorFor(request => request.AcceptedPolicyId, null as Guid?);
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
