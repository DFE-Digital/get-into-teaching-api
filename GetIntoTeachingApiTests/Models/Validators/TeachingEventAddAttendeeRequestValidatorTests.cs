using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class TeachingEventAddAttendeeRequestValidatorTests
    {
        private readonly TeachingEventAddAttendeeRequestValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public TeachingEventAddAttendeeRequestValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new TeachingEventAddAttendeeRequestValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };

            var request = new TeachingEventAddAttendeeRequest()
            {
                CandidateId = null,
                EventId = Guid.NewGuid(),
                AcceptedPolicyId = (Guid)mockPrivacyPolicy.Id,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                Telephone = "1234567",
                AddressPostcode = "KY11 9YU",
            };

            var result = _validator.TestValidate(request);

            // Ensure no validation errors on root object (we expect errors on the Candidate
            // properties as we can't mock them).
            var propertiesWithErrors = result.Errors.Select(e => e.PropertyName);
            propertiesWithErrors.All(p => p.StartsWith("Candidate.")).Should().BeTrue();
        }

        [Fact]
        public void Validate_CandidateIsInvalid_HasError()
        {
            var request = new TeachingEventAddAttendeeRequest
            {
                FirstName = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("Candidate.FirstName");
        }
    }
}
