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
    public class MailingListAddMemberValidatorTests
    {
        private readonly MailingListAddMemberValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public MailingListAddMemberValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new MailingListAddMemberValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new TypeEntity { Id = "123" };
            var mockEntityReference = new TypeEntity { Id = Guid.NewGuid().ToString() };
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };

            var request = new MailingListAddMember()
            {
                CandidateId = null,
                PreferredTeachingSubjectId = Guid.Parse(mockEntityReference.Id),
                AcceptedPolicyId = (Guid)mockPrivacyPolicy.Id,
                DescribeYourselfOptionId = int.Parse(mockPickListItem.Id),
                ConsiderationJourneyStageId = int.Parse(mockPickListItem.Id),
                DegreeStatusId = int.Parse(mockPickListItem.Id),
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
            var request = new MailingListAddMember
            {
                PreferredTeachingSubjectId = Guid.NewGuid(),
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("Candidate.PreferredTeachingSubjectId");
        }

        [Fact]
        public void Validate_FirstNameIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.FirstName, null as string);
        }

        [Fact]
        public void Validate_LastNameIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.LastName, null as string);
        }

        [Fact]
        public void Validate_EmailIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Email, null as string);
        }

        [Fact]
        public void Validate_AddressPostcodeIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AddressPostcode, null as string);
        }

        [Fact]
        public void Validate_AcceptedPolicyIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AcceptedPolicyId, null as Guid?);
        }
    }
}
