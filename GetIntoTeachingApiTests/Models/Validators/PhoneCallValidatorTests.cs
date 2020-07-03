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
    public class PhoneCallValidatorTests
    {
        private readonly PhoneCallValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public PhoneCallValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new PhoneCallValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new TypeEntity { Id = "123" };

            _mockStore
                .Setup(mock => mock.GetPickListItems("phonecall", "dfe_channelcreation"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetPickListItems("phonecall", "dfe_destination"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var phoneCall = new PhoneCall()
            {
                ScheduledAt = DateTime.Now.AddDays(2),
                ChannelId = int.Parse(mockPickListItem.Id),
                DestinationId = int.Parse(mockPickListItem.Id)
            };

            var result = _validator.TestValidate(phoneCall);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ScheduledAtInPast_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(phoneCall => phoneCall.ScheduledAt, DateTime.Now.AddDays(-1));
        }

        [Fact]
        public void Validate_ChannelIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(phoneCall => phoneCall.ChannelId, 123);
        }

        [Fact]
        public void Validate_ChannelIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(phoneCall => phoneCall.ChannelId, null as int?);
        }

        [Fact]
        public void Validate_DestinationIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(phoneCall => phoneCall.DestinationId, 123);
        }

        [Fact]
        public void Validate_DestinationIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(phoneCall => phoneCall.DestinationId, null as int?);
        }
    }
}
