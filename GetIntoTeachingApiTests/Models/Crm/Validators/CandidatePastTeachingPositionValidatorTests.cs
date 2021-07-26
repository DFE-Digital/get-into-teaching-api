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
            var mockPickListItem = new PickListItem { Id = 123 };
            var mockLookupItem = new LookupItem { Id = Guid.NewGuid() };

            _mockStore
                .Setup(mock => mock.GetLookupItems("dfe_teachingsubjectlist"))
                .Returns(new[] { mockLookupItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var position = new CandidatePastTeachingPosition
            {
                SubjectTaughtId = mockLookupItem.Id,
                EducationPhaseId = mockPickListItem.Id,
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
        public void Validate_SubjectTaughtIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(position => position.SubjectTaughtId, null as Guid?);
        }

        [Fact]
        public void Validate_EducationPhaseIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(position => position.EducationPhaseId, 123);
        }

        [Fact]
        public void Validate_EducationPhaseIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(position => position.EducationPhaseId, null as int?);
        }
    }
}
