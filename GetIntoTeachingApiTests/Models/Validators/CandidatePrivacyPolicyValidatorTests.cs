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
    public class CandidatePrivacyPolicyValidatorTests
    {
        private readonly CandidatePrivacyPolicyValidator _validator;
        private readonly Mock<ICrmService> _mockCrm;

        public CandidatePrivacyPolicyValidatorTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _validator = new CandidatePrivacyPolicyValidator(_mockCrm.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };

            _mockCrm
                .Setup(mock => mock.GetPrivacyPolicies())
                .Returns(new[] { mockPrivacyPolicy });

            var policy = new CandidatePrivacyPolicy()
            {
                AcceptedPolicy = mockPrivacyPolicy,
            };

            var result = _validator.TestValidate(policy);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_AcceptedPrivacyPolicyIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidatePolicy => candidatePolicy.AcceptedPolicy, null as PrivacyPolicy);
        }

        [Fact]
        public void Validate_AcceptedPrivacyPolicyIsInvalid_HasError()
        {
            var candidatePolicy = new CandidatePrivacyPolicy()
            {
                AcceptedPolicy = new PrivacyPolicy() { Id = Guid.NewGuid() }
            };
            var result = _validator.TestValidate(candidatePolicy);

            result.ShouldHaveValidationErrorFor("AcceptedPolicy.Id");
        }
    }
}
