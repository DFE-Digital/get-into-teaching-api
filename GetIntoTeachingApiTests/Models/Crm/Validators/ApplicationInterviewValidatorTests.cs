using System;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.Validators;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.Validators
{
    public class ApplicationInterviewValidatorTests
    {
        private readonly ApplicationInterviewValidator _validator;

        public ApplicationInterviewValidatorTests()
        {
            _validator = new ApplicationInterviewValidator();
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var interview = new ApplicationInterview()
            {
                ApplicationChoiceId = Guid.NewGuid(),
                FindApplyId = "67890",
            };

            var result = _validator.TestValidate(interview);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_RequiredAttributeIsEmpty_HasError()
        {
            var interview = new ApplicationInterview() { FindApplyId = "" };
            var result = _validator.TestValidate(interview);

            result.ShouldHaveValidationErrorFor("FindApplyId");
        }
    }
}
