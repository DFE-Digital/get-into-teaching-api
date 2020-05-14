using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class CandidateQualificationValidatorTests
    {
        private readonly CandidateQualificationValidator _validator;
        private readonly Mock<ICrmService> _mockCrm;

        public CandidateQualificationValidatorTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _validator = new CandidateQualificationValidator(_mockCrm.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockCategory = NewMock(111);
            var mockType = NewMock(222);
            var mockDegreeStatus = NewMock(333);

            _mockCrm
                .Setup(mock => mock.GetPickListItems("dfe_qualification", "dfe_category"))
                .Returns(new[] { mockCategory });
            _mockCrm
                .Setup(mock => mock.GetPickListItems("dfe_qualification", "dfe_type"))
                .Returns(new[] { mockType });
            _mockCrm
                .Setup(mock => mock.GetPickListItems("dfe_qualification", "dfe_degreestatus"))
                .Returns(new[] { mockDegreeStatus });

            var qualification = new CandidateQualification()
            {
                TypeId = mockType.Id,
                CategoryId = mockCategory.Id,
                DegreeStatusId = mockDegreeStatus.Id,
            };

            var result = _validator.TestValidate(qualification);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_TypeIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(qualification => qualification.TypeId, 123);
        }

        [Fact]
        public void Validate_TypeIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(qualification => qualification.TypeId, null as int?);
        }

        [Fact]
        public void Validate_CategoryIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(qualification => qualification.CategoryId, 123);
        }

        [Fact]
        public void Validate_CategoryIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(qualification => qualification.CategoryId, null as int?);
        }

        [Fact]
        public void Validate_DegreeStatusIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(qualification => qualification.DegreeStatusId, 123);
        }

        [Fact]
        public void Validate_DegreeStatusIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(qualification => qualification.DegreeStatusId, null as int?);
        }

        private TypeEntity NewMock(dynamic id)
        {
            return new TypeEntity { Id = id };
        }
    }
}
