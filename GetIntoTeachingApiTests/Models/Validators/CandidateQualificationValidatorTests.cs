using System.Linq;
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
        private readonly Mock<IStore> _mockStore;

        public CandidateQualificationValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new CandidateQualificationValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockCategory = NewMock(111);
            var mockType = NewMock(222);
            var mockDegreeStatus = NewMock(333);

            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_qualification", "dfe_category"))
                .Returns(new[] { mockCategory }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_qualification", "dfe_type"))
                .Returns(new[] { mockType }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_qualification", "dfe_degreestatus"))
                .Returns(new[] { mockDegreeStatus }.AsQueryable());

            var qualification = new CandidateQualification()
            {
                TypeId = int.Parse(mockType.Id),
                CategoryId = int.Parse(mockCategory.Id),
                DegreeStatusId = int.Parse(mockDegreeStatus.Id),
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

        private static TypeEntity NewMock(dynamic id)
        {
            return new TypeEntity { Id = id.ToString() };
        }
    }
}
