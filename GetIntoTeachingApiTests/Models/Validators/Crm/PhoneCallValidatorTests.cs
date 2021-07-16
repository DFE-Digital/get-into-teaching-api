using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Validators.Crm;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators.Crm
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
            var mockPickListItem = new PickListItem { Id = 123 };

            _mockStore
                .Setup(mock => mock.GetPickListItems("phonecall", "dfe_channelcreation"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var phoneCall = new PhoneCall()
            {
                ScheduledAt = DateTime.UtcNow.AddDays(2),
                ChannelId = mockPickListItem.Id,
            };

            var result = _validator.TestValidate(phoneCall);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ChannelIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(phoneCall => phoneCall.ChannelId, 123);
        }

        [Fact]
        public void Validate_ChannelIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(phoneCall => phoneCall.ChannelId, null as int?);
        }
    }
}
