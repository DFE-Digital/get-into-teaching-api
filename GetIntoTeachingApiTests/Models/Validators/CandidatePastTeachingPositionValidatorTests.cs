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
        private readonly Mock<IStore> _mockStore;

        public CandidatePastTeachingPositionValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new CandidatePastTeachingPositionValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockSubject = NewMock(Guid.NewGuid());
            var mockPhase = NewMock(123);

            _mockStore
                .Setup(mock => mock.GetLookupItems("dfe_teachingsubjectlist"))
                .Returns(new[] { mockSubject });
            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase"))
                .Returns(new[] { mockPhase });

            var position = new CandidatePastTeachingPosition
            {
                SubjectTaughtId = Guid.Parse(mockSubject.Id),
                EducationPhaseId = int.Parse(mockPhase.Id),
            };

            var result = _validator.TestValidate(position);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_SubjectTaughtIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(position => position.SubjectTaughtId, Guid.NewGuid());
        }

        [Fact]
        public void Validate_SubjectTaughtIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(position => position.SubjectTaughtId, null as Guid?);
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

        private static TypeEntity NewMock(dynamic id)
        {
            return new TypeEntity { Id = id.ToString() };
        }
    }
}
