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
    public class TeachingEventRegistrationValidatorTests
    {
        private readonly TeachingEventRegistrationValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public TeachingEventRegistrationValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new TeachingEventRegistrationValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockTeachingEvent = new TeachingEvent { Id = Guid.NewGuid(), WebFeedId = "123" };
            var mockPickListItem = new TypeEntity { Id = "123" };

            _mockStore
                .Setup(mock => mock.GetTeachingEventAsync((Guid)mockTeachingEvent.Id))
                .ReturnsAsync(mockTeachingEvent);
            _mockStore
                .Setup(mock => mock.GetPickListItems("msevtmgt_eventregistration", "dfe_channelcreation"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var registration = new TeachingEventRegistration() 
            {
                EventId = (Guid)mockTeachingEvent.Id,
                ChannelId = int.Parse(mockPickListItem.Id),
            };

            var result = _validator.TestValidate(registration);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EventIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(registration => registration.EventId, Guid.NewGuid());
        }

        [Fact]
        public void Validate_EventDoesNotAcceptOnlineRegistrations_HasError()
        {
            var mockTeachingEvent = new TeachingEvent { Id = Guid.NewGuid(), WebFeedId = null };
            var mockPickListItem = new TypeEntity { Id = "123" };

            _mockStore
                .Setup(mock => mock.GetTeachingEventAsync((Guid)mockTeachingEvent.Id))
                .ReturnsAsync(mockTeachingEvent);
            _mockStore
                .Setup(mock => mock.GetPickListItems("msevtmgt_eventregistration", "dfe_channelcreation"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var registration = new TeachingEventRegistration()
            {
                EventId = (Guid)mockTeachingEvent.Id,
                ChannelId = int.Parse(mockPickListItem.Id),
            };

            var result = _validator.TestValidate(registration);

            result.IsValid.Should().BeFalse();

            result.ShouldHaveValidationErrorFor("EventId")
                .WithErrorMessage("Attendence cannot be registered for this event via the API (it has no WebFeedId).");
        }

        [Fact]
        public void Validate_ChannelIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(registration => registration.ChannelId, 123);
        }

        [Fact]
        public void Validate_ChannelIdIsNullWhenExistingCandidate_HasNoError()
        {
            var registration = new TeachingEventRegistration() { Id = Guid.NewGuid(), ChannelId = null };
            var result = _validator.TestValidate(registration);

            result.ShouldNotHaveValidationErrorFor("ChannelId");
        }
    }
}
