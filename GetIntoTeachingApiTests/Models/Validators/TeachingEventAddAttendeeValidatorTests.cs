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
    public class TeachingEventAddAttendeeValidatorTests
    {
        private readonly TeachingEventAddAttendeeValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public TeachingEventAddAttendeeValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new TeachingEventAddAttendeeValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new TypeEntity { Id = "123" };
            var mockEntityReference = new TypeEntity { Id = Guid.NewGuid().ToString() };
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };

            var request = new TeachingEventAddAttendee()
            {
                CandidateId = null,
                EventId = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.Parse(mockEntityReference.Id),
                AcceptedPolicyId = (Guid)mockPrivacyPolicy.Id,
                ConsiderationJourneyStageId = int.Parse(mockPickListItem.Id),
                DegreeStatusId = int.Parse(mockPickListItem.Id),
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
            var request = new TeachingEventAddAttendee
            {
                FirstName = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("Candidate.FirstName");
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
        public void Validate_AcceptedPolicyIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AcceptedPolicyId, null as Guid?);
        }

        [Fact]
        public void Validate_EventIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.EventId, null as Guid?);
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
