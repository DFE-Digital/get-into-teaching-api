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
    public class ApplicationReferenceValidatorTests
    {
        private readonly ApplicationReferenceValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public ApplicationReferenceValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new ApplicationReferenceValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new PickListItem { Id = 123 };

            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_applyreference", "dfe_referencefeedbackstatus"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var reference = new ApplicationReference()
            {
                ApplicationFormId = Guid.NewGuid(),
                FindApplyId = "67890",
                Type = "Type",
                FeedbackStatusId = mockPickListItem.Id,
            };

            var result = _validator.TestValidate(reference);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_RequiredAttributeIsEmpty_HasError()
        {
            var reference = new ApplicationReference() { FindApplyId = "", Type = "" };
            var result = _validator.TestValidate(reference);

            result.ShouldHaveValidationErrorFor("FindApplyId");
            result.ShouldHaveValidationErrorFor("Type");
        }

        [Fact]
        public void Validate_OptionSetIsNotValid_HasError()
        {
            var reference = new ApplicationReference() { FeedbackStatusId = 456 };
            var result = _validator.TestValidate(reference);

            result.ShouldHaveValidationErrorFor("FeedbackStatusId");
        }

        [Fact]
        public void Validate_RequiredAttributeIsNull_HasError()
        {
            var reference = new ApplicationReference() { FeedbackStatusId = null };
            var result = _validator.TestValidate(reference);

            result.ShouldHaveValidationErrorFor("FeedbackStatusId");
        }
    }
}
