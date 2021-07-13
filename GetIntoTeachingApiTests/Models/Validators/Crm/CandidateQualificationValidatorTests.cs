using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Validators.Crm;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators.Crm
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
            var mockPickListItem = new PickListItem { Id = 123 };

            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_ukdegreegrade"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_degreestatus"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_type"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var qualification = new CandidateQualification()
            {
                UkDegreeGradeId = mockPickListItem.Id,
                DegreeSubject = "History",
                DegreeStatusId = mockPickListItem.Id,
                TypeId = mockPickListItem.Id,
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
        public void Validate_UkDegreeGradeIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(qualification => qualification.UkDegreeGradeId, null as int?);
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

        [Fact]
        public void Validate_TypeIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(qualification => qualification.TypeId, 123);
        }

        [Fact]
        public void Validate_TypeIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(qualification => qualification.TypeId, null as int?);
        }

        [Fact]
        public void Validate_SubjectTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(qualification => qualification.DegreeSubject, new string('a', 601));
        }
    }
}
