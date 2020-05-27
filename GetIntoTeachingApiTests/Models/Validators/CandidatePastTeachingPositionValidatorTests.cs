using System;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class CandidatePastTeachingPositionValidatorTests
    {
        private readonly CandidatePastTeachingPositionValidator _validator;
        private readonly Mock<ICrmService> _mockCrm;

        public CandidatePastTeachingPositionValidatorTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _validator = new CandidatePastTeachingPositionValidator(_mockCrm.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockTeachingSubject = new TypeEntity { Id = Guid.NewGuid() };
            var mockPhase = new TypeEntity { Id = 123 };

            _mockCrm
                .Setup(mock => mock.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase"))
                .Returns(new[] { mockPhase });
            _mockCrm
                .Setup(mock => mock.GetLookupItems("dfe_teachingsubjectlist"))
                .Returns(new[] { mockTeachingSubject });

            var position = new CandidatePastTeachingPosition
            {
                EducationPhaseId = mockPhase.Id,
                SubjectTaught = new TeachingSubject() { Id = mockTeachingSubject.Id }
            };

            var result = _validator.TestValidate(position);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_SubjectTaughtIsInvalid_HasError()
        {
            var position = new CandidatePastTeachingPosition()
            {
                SubjectTaught = new TeachingSubject() { Id = Guid.NewGuid() }
            };
            var result = _validator.TestValidate(position);

            result.ShouldHaveValidationErrorFor("SubjectTaught.Id");
        }

        [Fact]
        public void Validate_SubjectTaughtIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(position => position.SubjectTaught, null as TeachingSubject);
        }

        [Fact]
        public void Validate_EducationPhaseIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(position => position.EducationPhaseId, 123);
        }

        [Fact]
        public void Validate_EducationPhaseIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(position => position.EducationPhaseId, null as int?);
        }
    }
}
