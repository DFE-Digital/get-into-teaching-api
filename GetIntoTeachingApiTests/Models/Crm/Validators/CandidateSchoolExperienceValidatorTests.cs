using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.Validators
{
    public class CandidateSchoolExperienceValidatorTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly CandidateSchoolExperienceValidator _validator;

        public CandidateSchoolExperienceValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new CandidateSchoolExperienceValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockTeachingSubject = new LookupItem { Id = Guid.NewGuid() };

            _mockStore
               .Setup(mock => mock.GetLookupItems("dfe_teachingsubjectlist"))
               .Returns(new[] { mockTeachingSubject }.AsQueryable());

            var candidateSchoolExperience = new CandidateSchoolExperience()
            {
                SchoolUrn = "123456",
                DurationOfPlacementInDays = 1,
                TeachingSubjectId = mockTeachingSubject.Id,
                Notes = "Notes about the candidate.",
                SchoolName = "James Brindley High School"
            };

            var result = _validator.TestValidate(candidateSchoolExperience);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_SchoolUrnIsInvalid_HasError()
        {
            var candidateSchoolExperience = new CandidateSchoolExperience()
            {
                SchoolUrn = "123456789",
            };

            var result = _validator.TestValidate(candidateSchoolExperience);

            result.ShouldHaveValidationErrorFor(c => c.SchoolUrn);
        }

        [Fact]
        public void Validate_DurationOfPlacementInDaysTooMany_HasError()
        {
            var candidateSchoolExperience = new CandidateSchoolExperience()
            {
                DurationOfPlacementInDays = 101,
            };

            var result = _validator.TestValidate(candidateSchoolExperience);

            result.ShouldHaveValidationErrorFor(c => c.DurationOfPlacementInDays);
        }

        [Fact]
        public void Validate_TeachingSubjectIdNotFound_HasError()
        {
            _mockStore
              .Setup(mock => mock.GetLookupItems("dfe_teachingsubjectlist"))
              .Returns(new List<LookupItem>().AsQueryable());
            var candidateSchoolExperience = new CandidateSchoolExperience()
            {
                TeachingSubjectId = Guid.NewGuid(),
            };

            var result = _validator.TestValidate(candidateSchoolExperience);

            result.ShouldHaveValidationErrorFor(c => c.TeachingSubjectId);
        }

        [Fact]
        public void Validate_NotesTooLong_HasError()
        {
            var candidateSchoolExperience = new CandidateSchoolExperience()
            {
                Notes = new string('*', 2001),
            };

            var result = _validator.TestValidate(candidateSchoolExperience);

            result.ShouldHaveValidationErrorFor(c => c.Notes);
        }

        [Fact]
        public void Validate_SchoolNameTooLong_HasError()
        {
            var candidateSchoolExperience = new CandidateSchoolExperience()
            {
                SchoolName = new string('*', 101),
            };

            var result = _validator.TestValidate(candidateSchoolExperience);

            result.ShouldHaveValidationErrorFor(c => c.SchoolName);
        }
    }
}
