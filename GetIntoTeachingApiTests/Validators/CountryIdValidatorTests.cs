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
    public class CountryIdValidatorTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly Country _item;
        private readonly CountryIdValidator<object> _validator;

        public CountryIdValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new CountryIdValidator<object>(_mockStore.Object);
            _item = new Country() { Id = Guid.NewGuid() };

            _mockStore.Setup(m => m.GetCountries()).Returns(new List<Country>() { _item }.AsQueryable());
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
