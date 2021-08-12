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
    public class PickListItemIdsValidatorTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly List<PickListItem> _items;
        private readonly PickListItemIdsValidator<object> _validator;

        public PickListItemIdsValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new PickListItemIdsValidator<object>("contact", "dfe_channel", _mockStore.Object);
            _items = new List<PickListItem> {
                new PickListItem { Id = 111 },
                new PickListItem { Id = 222 }
            };

            _mockStore.Setup(m => m.GetPickListItems("contact", "dfe_channel")).Returns(_items.AsQueryable());
        }

        [Theory]
        [InlineData(111)]
        [InlineData(111, 222)]
        public void IsValid_WhenValidIds_ReturnsTrue(params int[] ids)
        {
            var context = new ValidationContext<object>(this);

            var valid = _validator.IsValid(context, ids);

            valid.Should().BeTrue();
        }

        [Theory]
        [InlineData(333)]
        [InlineData(111, 333)]
        public void IsValid_WhenInvalidIds_ReturnsFalse(params int[] ids)
        {
            var context = new ValidationContext<object>(this);

            var valid = _validator.IsValid(context, ids);

            valid.Should().BeFalse();
        }
    }
}
