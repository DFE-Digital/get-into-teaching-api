using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Models.GetIntoTeaching.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching.Validators
{
    public class TeachingEventAddAttendeeValidatorTests
    {
        private readonly TeachingEventAddAttendeeValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public TeachingEventAddAttendeeValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new TeachingEventAddAttendeeValidator(_mockStore.Object, new DateTimeProvider());
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new PickListItem { Id = 123 };
            var mockLookupItem = new LookupItem { Id = Guid.NewGuid() };
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };

            var request = new TeachingEventAddAttendee()
            {
                CandidateId = null,
                EventId = Guid.NewGuid(),
                PreferredTeachingSubjectId = mockLookupItem.Id,
                AcceptedPolicyId = (Guid)mockPrivacyPolicy.Id,
                ConsiderationJourneyStageId = mockPickListItem.Id,
                DegreeStatusId = mockPickListItem.Id,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                AddressTelephone = "1234567",
                AddressPostcode = "KY11 9YU",
                IsWalkIn = true,
                IsVerified = false,
            };

            var result = _validator.TestValidate(request);

            // Ensure no validation errors on root object (we expect errors on the Candidate
            // properties as we can't mock them).
            var propertiesWithErrors = result.Errors.Select(e => e.PropertyName);
            propertiesWithErrors.All(p => p.StartsWith("Candidate.")).Should().BeTrue();
        }

        [Fact]
        public void Validate_IsNotVerifiedAndIsNotWalkIn_HasError()
        {
            var request = new TeachingEventAddAttendee() { IsWalkIn = false, IsVerified = false };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("IsVerified");

            request.IsVerified = true;

            result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("IsVerified");
        }

        [Fact]
        public void Validate_CandidateIsInvalid_HasError()
        {
            var request = new TeachingEventAddAttendee
            {
                Email = "invalid@"
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("Candidate.Email");
        }


        [Fact]
        public void Validate_RequiredFieldsWhenNull_HasError()
        {
            var attendee = new TeachingEventAddAttendee()
            {
                FirstName = null,
                LastName = null,
                Email = null,
                AcceptedPolicyId = null,
                EventId = null,
            };
            var result = _validator.TestValidate(attendee);

            result.ShouldHaveValidationErrorFor(a => a.FirstName);
            result.ShouldHaveValidationErrorFor(a => a.LastName);
            result.ShouldHaveValidationErrorFor(a => a.Email);
            result.ShouldHaveValidationErrorFor(a => a.AcceptedPolicyId);
            result.ShouldHaveValidationErrorFor(a => a.EventId);
        }

        [Fact]
        public void Validate_ConsiderationJourneyStageIdIsNullWhenSigningUpToMailingList_HasError()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = true, ConsiderationJourneyStageId = null };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request.ConsiderationJourneyStageId);
        }

        [Fact]
        public void Validate_ConsiderationJourneyStageIdIsNullWhenNotSigningUpToMailingList_HasNoError()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = false, ConsiderationJourneyStageId = null };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request.ConsiderationJourneyStageId);
        }

        [Fact]
        public void Validate_DegreeStatusIdIsNullWhenSigningUpToMailingList_HasError()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = true, DegreeStatusId = null };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request.DegreeStatusId);
        }

        [Fact]
        public void Validate_DegreeStatusIdIsNullWhenNotSigningUpToMailingList_HasNoError()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = false, DegreeStatusId = null };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request.DegreeStatusId);
        }

        [Fact]
        public void Validate_PreferredTeachingSubjectIdIsNullWhenSigningUpToMailingList_HasError()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = true, PreferredTeachingSubjectId = null };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request.PreferredTeachingSubjectId);
        }

        [Fact]
        public void Validate_PreferredTeachingSubjectIdIsNullWhenNotSigningUpToMailingList_HasNoError()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = false, PreferredTeachingSubjectId = null };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request.PreferredTeachingSubjectId);
        }
    }
}
