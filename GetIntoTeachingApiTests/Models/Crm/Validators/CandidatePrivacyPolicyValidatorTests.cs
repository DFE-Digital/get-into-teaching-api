﻿using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.Validators
{
    public class CandidatePrivacyPolicyValidatorTests
    {
        private readonly CandidatePrivacyPolicyValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public CandidatePrivacyPolicyValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new CandidatePrivacyPolicyValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };

            _mockStore
                .Setup(mock => mock.GetPrivacyPolicies())
                .Returns(new[] { mockPrivacyPolicy }.AsQueryable());

            var policy = new CandidatePrivacyPolicy()
            {
                AcceptedPolicyId = (Guid)mockPrivacyPolicy.Id,
            };

            var result = _validator.TestValidate(policy);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_AcceptedPolicyIdIsInvalid_HasError()
        {
            var result = _validator.TestValidate(new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() });

            result.ShouldHaveValidationErrorFor(p => p.AcceptedPolicyId);
        }
    }
}
