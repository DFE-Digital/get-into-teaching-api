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
    public class TeachingSubjectIdValidatorTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly TeachingSubject _item;
        private readonly TeachingSubjectIdValidator<object> _validator;

        public TeachingSubjectIdValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new TeachingSubjectIdValidator<object>(_mockStore.Object);
            _item = new TeachingSubject() { Id = Guid.NewGuid() };

            _mockStore.Setup(m => m.GetTeachingSubjects()).Returns(new List<TeachingSubject>() { _item }.AsQueryable());
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
