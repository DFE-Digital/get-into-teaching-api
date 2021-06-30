using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;
using Moq;
using Xunit;
using System;
using FluentValidation;
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
        private readonly LookupItemIdValidator<object> _validator;

        public LookupItemIdValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new LookupItemIdValidator<object>("dfe_country", _mockStore.Object);
            _item = new LookupItem() { Id = Guid.NewGuid() };

            _mockStore.Setup(m => m.GetLookupItems("dfe_country")).Returns(new List<LookupItem>() { _item }.AsQueryable());
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

            var valid = _validator.IsValid(context, Guid.NewGuid());

            valid.Should().BeFalse();
        }
    }
}
