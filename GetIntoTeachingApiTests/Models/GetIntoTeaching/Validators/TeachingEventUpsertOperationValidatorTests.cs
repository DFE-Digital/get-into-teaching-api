using System;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Models.GetIntoTeaching.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching.Validators
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

            var result = _validator.TestValidate(operation);

            result.ShouldNotHaveValidationErrorFor(te => te.ReadableId);
        }

        [Fact]
        public void Validate_WhenExistingEventWithReadableIdHasSameTeachingEventId_HasNoErrors()
        {
            var operation = new TeachingEventUpsertOperation() { Id = Guid.NewGuid(), ReadableId = "existing" };
            var existingTeachingEvent = new TeachingEvent() { Id = operation.Id };

            _mockCrm.Setup(m => m.GetTeachingEvent("existing")).Returns(existingTeachingEvent);

            var result = _validator.TestValidate(operation);

            result.ShouldNotHaveValidationErrorFor(te => te.ReadableId);
        }

        [Fact]
        public void Validate_WhenExistingEventWithReadableIdHasDifferentTeachingEventId_HasError()
        {
            var operation = new TeachingEventUpsertOperation() { Id = Guid.NewGuid(), ReadableId = "existing" };
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid() };

            _mockCrm.Setup(m => m.GetTeachingEvent("existing")).Returns(teachingEvent);

            var result = _validator.TestValidate(operation);

            result.ShouldHaveValidationErrorFor(te => te.ReadableId)
                .WithErrorMessage("Must be unique");
        }

        [Theory]
        [InlineData("eventname", false)]
        [InlineData("eventname1", false)]
        [InlineData("event-name", false)]
        [InlineData("event_name", false)]
        [InlineData("event name", true)]
        [InlineData("event?name", true)]
        [InlineData("event@name", true)]
        [InlineData("event:name", true)]
        public void Validate_ReadableId_AllowsOnlyAlphanumericHyphensUnderscores(string readableId, bool hasError)
        {
            var operation = new TeachingEventUpsertOperation() { ReadableId = readableId };
            var result = _validator.TestValidate(operation);

            if (hasError)
            {
                result.ShouldHaveValidationErrorFor(te => te.ReadableId);
            }
            else
            {
                result.ShouldNotHaveValidationErrorFor(te => te.ReadableId);
            }
        }
    }
}
