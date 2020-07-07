using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class MailingListAddMemberRequestValidatorTests
    {
        private readonly MailingListAddMemberRequestValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public MailingListAddMemberRequestValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new MailingListAddMemberRequestValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new TypeEntity { Id = "123" };
            var mockEntityReference = new TypeEntity { Id = Guid.NewGuid().ToString() };
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };

            var request = new MailingListAddMemberRequest()
            {
                CandidateId = null,
                PreferredTeachingSubjectId = Guid.Parse(mockEntityReference.Id),
                AcceptedPolicyId = (Guid)mockPrivacyPolicy.Id,
                DescribeYourselfOptionId = int.Parse(mockPickListItem.Id),
                ConsiderationJourneyStageId = int.Parse(mockPickListItem.Id),
                UkDegreeGradeId = int.Parse(mockPickListItem.Id),
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                Telephone = "1234567",
                AddressPostcode = "KY11 9YU",
                CallbackInformation = "Test information",
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
            var request = new MailingListAddMemberRequest
            {
                PreferredTeachingSubjectId = Guid.NewGuid(),
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("Candidate.PreferredTeachingSubjectId");
        }

        [Fact]
        public void Validate_AddressPostcodeIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AddressPostcode, null as string);
        }
    }
}
