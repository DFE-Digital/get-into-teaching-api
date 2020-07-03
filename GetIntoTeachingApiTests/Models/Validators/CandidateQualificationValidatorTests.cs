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
            var mockGrade = NewMock(111);
            var mockDegreeStatus = NewMock(333);
            var mockType = NewMock(333);

            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_ukdegreegrade"))
                .Returns(new[] { mockGrade }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_degreestatus"))
                .Returns(new[] { mockDegreeStatus }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_type"))
                .Returns(new[] { mockType }.AsQueryable());

            var qualification = new CandidateQualification()
            {
                UkDegreeGradeId = int.Parse(mockGrade.Id),
                Subject = "History",
                DegreeStatusId = int.Parse(mockDegreeStatus.Id),
                TypeId = int.Parse(mockType.Id),
            };

            var result = _validator.TestValidate(qualification);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_UkDegreeGradeIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(qualification => qualification.UkDegreeGradeId, 123);
        }

        [Fact]
        public void Validate_UkDegreeGradeIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(qualification => qualification.UkDegreeGradeId, null as int?);
        }

        [Fact]
        public void Validate_DegreeStatusIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(qualification => qualification.DegreeStatusId, 123);
        }

        [Fact]
        public void Validate_DegreeStatusIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(qualification => qualification.DegreeStatusId, null as int?);
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
        public void Validate_SubjectTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(qualification => qualification.Subject, new string('a', 601));
        }

        private static TypeEntity NewMock(dynamic id)
        {
            return new TypeEntity { Id = id.ToString() };
        }
    }
}
