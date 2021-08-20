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
    public class ApplicationFormValidatorTests
    {
        private readonly ApplicationFormValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public ApplicationFormValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new ApplicationFormValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new PickListItem { Id = 123 };

            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_applyapplicationform", "dfe_candidateapplyphase"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var form = new ApplicationForm()
            {
                CandidateId = Guid.NewGuid(),
                FindApplyId = "67890",
                PhaseId = mockPickListItem.Id,
            };

            var result = _validator.TestValidate(form);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_FindApplyIdIsEmpty_HasError()
        {
            var form = new ApplicationForm() { FindApplyId = "" };
            var result = _validator.TestValidate(form);

            result.ShouldHaveValidationErrorFor("FindApplyId");
        }

        [Fact]
        public void Validate_PhaseIdIsNotValid_HasError()
        {
            var form = new ApplicationForm() { PhaseId = 456 };
            var result = _validator.TestValidate(form);

            result.ShouldHaveValidationErrorFor("PhaseId");
        }
    }
}
