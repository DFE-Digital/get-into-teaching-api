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
        public void Validate_IdFieldWithInvalidPickListItemId_HasError()
        {
            var qualification = new CandidateQualification()
            {
                UkDegreeGradeId = 123,
                DegreeStatusId = 123,
                TypeId = 123,
            };
            var result = _validator.TestValidate(qualification);

            result.ShouldHaveValidationErrorFor(c => c.UkDegreeGradeId);
            result.ShouldHaveValidationErrorFor(c => c.DegreeStatusId);
            result.ShouldHaveValidationErrorFor(c => c.TypeId);
        }

        [Fact]
        public void Validate_SubjectTooLong_HasError()
        {
            var result = _validator.TestValidate(new CandidateQualification() { DegreeSubject = new string('a', 601) });

            result.ShouldHaveValidationErrorFor(c => c.DegreeSubject);
        }
    }
}
