using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Models.GetIntoTeaching.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching.Validators
{
    public class MailingListAddMemberValidatorTests
    {
        private readonly MailingListAddMemberValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public MailingListAddMemberValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new MailingListAddMemberValidator(_mockStore.Object, new DateTimeProvider());
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new PickListItem { Id = 123 };
            var mockLookupItem = new LookupItem { Id = Guid.NewGuid() };
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };

            var request = new MailingListAddMember()
            {
                CandidateId = null,
                PreferredTeachingSubjectId = mockLookupItem.Id,
                AcceptedPolicyId = (Guid)mockPrivacyPolicy.Id,
                ConsiderationJourneyStageId = mockPickListItem.Id,
                DegreeStatusId = mockPickListItem.Id,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
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
        public void Validate_AddressPostcodeIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(request => request.AddressPostcode, null as string);
        }

        [Fact]
        public void Validate_AcceptedPolicyIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AcceptedPolicyId, null as Guid?);
        }

        [Fact]
        public void Validate_ConsiderationJourneyStageIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.ConsiderationJourneyStageId, null as int?);
        }

        [Fact]
        public void Validate_DegreeStatusIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.DegreeStatusId, null as int?);
        }

        [Fact]
        public void Validate_PreferredTeachingSubjectIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.PreferredTeachingSubjectId, null as Guid?);
        }
    }
}
