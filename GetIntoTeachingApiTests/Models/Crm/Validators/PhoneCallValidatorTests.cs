using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.Validators
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
            var result = _validator.TestValidate(new PhoneCall() { ChannelId = 123 });

            result.ShouldHaveValidationErrorFor(r => r.ChannelId);
        }
    }
}
