using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;
using Moq;
using Xunit;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using GetIntoTeachingApi.Models;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace GetIntoTeachingApiTests.Validators
{
    public class PickListItemIdValidatorTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly PickListItem _item;
        private readonly PickListItemIdValidator _validator;

        public PickListItemIdValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new PickListItemIdValidator("contact", "dfe_channel", _mockStore.Object);
            _item = new PickListItem() { Id = 123 };

            _mockStore.Setup(m => m.GetPickListItems("contact", "dfe_channel")).Returns(new List<PickListItem>() { _item }.AsQueryable());
        }

        [Fact]
        public void IsValid_WhenValidId_ReturnsTrue()
        {
            var selector = ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory();
            var validationContext = new ValidationContext(_item.Id, new PropertyChain(), selector);
            var propertyValidatorContext = new PropertyValidatorContext(validationContext, PropertyRule.Create<int?, int?>(t => t), "prop");

            var errors = _validator.Validate(propertyValidatorContext);

            errors.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_WhenInvalidId_ReturnsFalse()
        {
            var selector = ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory();
            var validationContext = new ValidationContext(456, new PropertyChain(), selector);
            var propertyValidatorContext = new PropertyValidatorContext(validationContext, PropertyRule.Create<int?, int?>(t => t), "prop");

            var errors = _validator.Validate(propertyValidatorContext);

            errors.Should().NotBeEmpty();
        }
    }
}
