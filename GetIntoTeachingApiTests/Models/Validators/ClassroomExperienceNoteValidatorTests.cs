using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
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
        public void Validate_ActionIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Action, "");
        }

        [Fact]
        public void Validate_RecordedAtIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.RecordedAt, null as DateTime?);
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
            if (hasError)
            {
                _validator.ShouldHaveValidationErrorFor(request => request.Action, action);
            }
            else
            {
                _validator.ShouldNotHaveValidationErrorFor(request => request.Action, action);
            }
        }

        [Fact]
        public void Validate_SchoolNameIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.SchoolName, "");
        }

        [Fact]
        public void Validate_SchoolUrnIsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.SchoolUrn, 1234567);
        }

        [Fact]
        public void Validate_SchoolUrnIsWrongLength_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.SchoolUrn, 11);
        }

        [Fact]
        public void Validate_SchoolUrnIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.SchoolUrn, null as int?);
        }
    }
}
