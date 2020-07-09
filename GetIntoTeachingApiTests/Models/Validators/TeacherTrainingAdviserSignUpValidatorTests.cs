using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class TeacherTrainingAdviserSignUpValidatorTests
    {
        private readonly TeacherTrainingAdviserSignUpValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public TeacherTrainingAdviserSignUpValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new TeacherTrainingAdviserSignUpValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var request = new TeacherTrainingAdviserSignUp()
            {
                CandidateId = Guid.NewGuid(),
                AcceptedPolicyId = Guid.NewGuid(),
                QualificationId = Guid.NewGuid(),
                SubjectTaughtId = Guid.NewGuid(),
                PastTeachingPositionId = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                CountryId = Guid.NewGuid(),
                UkDegreeGradeId = 1,
                DegreeStatusId = 2,
                InitialTeacherTrainingYearId = 3,
                PreferredEducationPhaseId = 4,
                HasGcseEnglishId = 5,
                HasGcseMathsId = 6,
                HasGcseScienceId = 7,
                PlanningToRetakeCgseScienceId = 8,
                PlanningToRetakeGcseEnglishId = 9,
                PlanningToRetakeGcseMathsId = 10,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now,
                TeacherId = "abc123",
                DegreeSubject = "Maths",
                Telephone = "1234567",
                AddressLine1 = "Line 1",
                AddressLine2 = "Line 2",
                AddressLine3 = "Line 3",
                AddressCity = "City",
                AddressState = "State",
                AddressPostcode = "KY11 9YU",
                PhoneCallScheduledAt = DateTime.Now,
            };

            var result = _validator.TestValidate(request);

            // Ensure no validation errors on root object (we expect errors on the Candidate
            // properties as we can't mock them).
            var propertiesWithErrors = result.Errors.Select(e => e.PropertyName);
            propertiesWithErrors.All(p => p.StartsWith("Candidate.")).Should().BeTrue();
        }

        [Fact]
        public void Validate_CandidateIsInvalid_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                FirstName = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("Candidate.FirstName");
        }
    }
}
