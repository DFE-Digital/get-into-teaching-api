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
    public class ApplicationChoiceValidatorTests
    {
        private readonly ApplicationChoiceValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public ApplicationChoiceValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new ApplicationChoiceValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new PickListItem { Id = 123 };

            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_applyapplicationchoice", "dfe_applicationchoicestatus"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var choice = new ApplicationChoice()
            {
                ApplicationFormId = Guid.NewGuid(),
                FindApplyId = "67890",
                CourseName = "Course Name",
                Provider = "Course Provider",
                CourseId = Guid.NewGuid().ToString(),
                StatusId = mockPickListItem.Id,
            };

            var result = _validator.TestValidate(choice);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_RequiredAttributeIsEmpty_HasError()
        {
            var choice = new ApplicationChoice() { FindApplyId = "", CourseId = "", CourseName = "", Provider = "" };
            var result = _validator.TestValidate(choice);

            result.ShouldHaveValidationErrorFor("FindApplyId");
            result.ShouldHaveValidationErrorFor("CourseId");
            result.ShouldHaveValidationErrorFor("CourseName");
            result.ShouldHaveValidationErrorFor("Provider");
        }

        [Fact]
        public void Validate_CourseIdIsNotGuid_HasError()
        {
            var choice = new ApplicationChoice() { CourseId = "abc-123-zzz" };
            var result = _validator.TestValidate(choice);

            result.ShouldHaveValidationErrorFor("CourseId");
        }

        [Fact]
        public void Validate_OptionSetIsNotValid_HasError()
        {
            var choice = new ApplicationChoice() { StatusId = 456 };
            var result = _validator.TestValidate(choice);

            result.ShouldHaveValidationErrorFor("StatusId");
        }

        [Fact]
        public void Validate_RequiredAttributeIsNull_HasError()
        {
            var choice = new ApplicationChoice() { StatusId = null };
            var result = _validator.TestValidate(choice);

            result.ShouldHaveValidationErrorFor("StatusId");
        }
    }
}
