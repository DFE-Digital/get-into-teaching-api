using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;
using Moq;
using Xunit;
using FluentValidation;
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
        private readonly PickListItemIdValidator<object> _validator;

        public PickListItemIdValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new PickListItemIdValidator<object>("contact", "dfe_channel", _mockStore.Object);
            _item = new PickListItem() { Id = 123 };

            _mockStore.Setup(m => m.GetPickListItems("contact", "dfe_channel")).Returns(new List<PickListItem>() { _item }.AsQueryable());
        }

        [Fact]
        public void IsValid_WhenValidId_ReturnsTrue()
        {
            var context = new ValidationContext<object>(this);

            var valid = _validator.IsValid(context, _item.Id);

            valid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WhenInvalidId_ReturnsFalse()
        {
            var context = new ValidationContext<object>(this);

            var valid = _validator.IsValid(context, 456);

            valid.Should().BeFalse();
        }
    }
}
