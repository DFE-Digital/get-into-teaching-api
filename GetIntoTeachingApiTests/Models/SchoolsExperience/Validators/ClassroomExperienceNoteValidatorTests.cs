using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models.SchoolsExperience;
using GetIntoTeachingApi.Models.SchoolsExperience.Validators;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.SchoolsExperience.Validators
{
    public class ClassroomExperienceNoteValidatorTests
    {
        private readonly ClassroomExperienceNoteValidator _validator;

        public ClassroomExperienceNoteValidatorTests()
        {
            _validator = new ClassroomExperienceNoteValidator();
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var request = new ClassroomExperienceNote
            {
                Action = "REQUEST",
                RecordedAt = DateTime.UtcNow.AddDays(-5),
                Date = DateTime.UtcNow,
                SchoolName = "John Reed Primary",
                SchoolUrn = 123456,
            };

            var result = _validator.TestValidate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_RequiredFieldsWhenNullOrEmpty_HasError()
        {
            var note = new ClassroomExperienceNote()
            {
                Action = "",
                RecordedAt = null,
                SchoolName = "",
                SchoolUrn = null,

            };
            var result = _validator.TestValidate(note);

            result.ShouldHaveValidationErrorFor(s => s.Action);
            result.ShouldHaveValidationErrorFor(s => s.RecordedAt);
            result.ShouldHaveValidationErrorFor(s => s.SchoolName);
            result.ShouldHaveValidationErrorFor(s => s.SchoolUrn);
        }

        [Theory]
        [InlineData("REQUEST", false)]
        [InlineData("ACCEPTED", false)]
        [InlineData("ATTENDED", false)]
        [InlineData("DID NOT ATTEND", false)]
        [InlineData("CANCELLED BY SCHOOL", false)]
        [InlineData("CANCELLED BY CANDIDATE", false)]
        [InlineData("REQ", true)]
        [InlineData("NOT ATTEND", true)]
        [InlineData("CANCELLED BY", true)]
        [InlineData("CANCELLED BY JOHN", true)]
        public void Validate_ActionFormat_ValidatesCorrectly(string action, bool hasError)
        {
            var note = new ClassroomExperienceNote() { Action = action };
            var result = _validator.TestValidate(note);

            if (hasError)
            {
                result.ShouldHaveValidationErrorFor(s => s.Action);
            }
            else
            {
                result.ShouldNotHaveValidationErrorFor(s => s.Action);
            }
        }

        [Fact]
        public void Validate_SchoolUrnIsTooLong_HasError()
        {
            var note = new ClassroomExperienceNote() { SchoolUrn = 1234567 };
            var result = _validator.TestValidate(note);

            result.ShouldHaveValidationErrorFor(s => s.Action);
        }

        [Fact]
        public void Validate_SchoolUrnIsTooShort_HasError()
        {
            var note = new ClassroomExperienceNote() { SchoolUrn = 11 };
            var result = _validator.TestValidate(note);

            result.ShouldHaveValidationErrorFor(s => s.Action);
        }
    }
}
