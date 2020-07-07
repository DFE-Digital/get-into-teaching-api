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
            var mockTeachingEvent = new TeachingEvent { Id = Guid.NewGuid() };

            _mockStore
                .Setup(mock => mock.GetTeachingEventAsync((Guid)mockTeachingEvent.Id))
                .ReturnsAsync(mockTeachingEvent);

            var registration = new TeachingEventRegistration() { EventId = (Guid)mockTeachingEvent.Id };

            var result = _validator.TestValidate(registration);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EventIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(registration => registration.EventId, Guid.NewGuid());
        }
    }
}
