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
    public class ServiceSubscriptionValidatorTests
    {
        private readonly ServiceSubscriptionValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public ServiceSubscriptionValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new ServiceSubscriptionValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new TypeEntity { Id = "123" };

            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_servicesubscription", "dfe_servicesubscriptiontype"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var subscription = new ServiceSubscription()
            {
                TypeId = int.Parse(mockPickListItem.Id),
                DoNotBulkEmail = false,
                DoNotBulkPostalMail = true,
                DoNotEmail = true,
                DoNotPostalMail = false,
                DoNotSendMm = true,
                OptOutOfSms = false,
            };

            var result = _validator.TestValidate(subscription);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_TypeIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(subscription => subscription.TypeId, 123);
        }

        [Fact]
        public void Validate_TypeIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(subscription => subscription.TypeId, null as int?);
        }
    }
}
