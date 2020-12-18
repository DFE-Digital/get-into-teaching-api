using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;
using Moq;
using Xunit;
using System;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using GetIntoTeachingApi.Models;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace GetIntoTeachingApiTests.Validators
{
    public class LookupItemIdValidatorTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly LookupItem _item;
        private readonly LookupItemIdValidator _validator;

        public LookupItemIdValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new LookupItemIdValidator("dfe_country", _mockStore.Object);
            _item = new LookupItem() { Id = Guid.NewGuid() };

            _mockStore.Setup(m => m.GetLookupItems("dfe_country")).Returns(new List<LookupItem>() { _item }.AsQueryable());
        }

        [Fact]
        public void IsValid_WhenValidId_ReturnsTrue()
        {
            var selector = ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory();
            var validationContext = new ValidationContext(_item.Id, new PropertyChain(), selector);
            var propertyValidatorContext = new PropertyValidatorContext(validationContext, PropertyRule.Create<Guid, Guid>(t => t), "Prop");

            var errors = _validator.Validate(propertyValidatorContext);

            errors.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_WhenInvalidId_ReturnsFalse()
        {
            var selector = ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory();
            var validationContext = new ValidationContext(Guid.NewGuid(), new PropertyChain(), selector);
            var propertyValidatorContext = new PropertyValidatorContext(validationContext, PropertyRule.Create<Guid, Guid>(t => t), "Prop");

            var errors = _validator.Validate(propertyValidatorContext);

            errors.Should().NotBeEmpty();
            errors.First().ErrorMessage.Should().Equals("Prop must be a valid dfe_country item.");
        }
    }
}
