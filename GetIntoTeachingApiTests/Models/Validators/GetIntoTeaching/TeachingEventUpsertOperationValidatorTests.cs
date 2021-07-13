using System;
using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Models.Validators.GetIntoTeaching;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators.GetIntoTeaching
{
    public class TeachingEventUpsertOperationValidatorTests
    {
        private readonly TeachingEventUpsertOperationValidator _validator;
        private readonly Mock<ICrmService> _mockCrm;

        public TeachingEventUpsertOperationValidatorTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _validator = new TeachingEventUpsertOperationValidator(_mockCrm.Object);
        }

        [Fact]
        public void Validate_WhenExistingEventWithReadableIdIsNotFound_HasNoErrors()
        {
            var operation = new TeachingEventUpsertOperation() { ReadableId = "new" };

            _mockCrm.Setup(m => m.GetTeachingEvent("new")).Returns<TeachingEvent>(null);

            var result = _validator.Validate(operation);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_WhenExistingEventWithReadableIdHasSameTeachingEventId_HasNoErrors()
        {
            var operation = new TeachingEventUpsertOperation() { Id = Guid.NewGuid(), ReadableId = "existing" };
            var existingTeachingEvent = new TeachingEvent() { Id = operation.Id };

            _mockCrm.Setup(m => m.GetTeachingEvent("existing")).Returns(existingTeachingEvent);

            var result = _validator.Validate(operation);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_WhenExistingEventWithReadableIdHasDifferentTeachingEventId_HasErrors()
        {
            var operation = new TeachingEventUpsertOperation() { Id = Guid.NewGuid(), ReadableId = "existing" };
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid() };

            _mockCrm.Setup(m => m.GetTeachingEvent("existing")).Returns(teachingEvent);

            var result = _validator.Validate(operation);

            result.IsValid.Should().BeFalse();
        }
    }
}
