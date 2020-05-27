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
    public class TeachingSubjectValidatorTests
    {
        private readonly TeachingSubjectValidator _validator;
        private readonly Mock<ICrmService> _mockCrm;

        public TeachingSubjectValidatorTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _validator = new TeachingSubjectValidator(_mockCrm.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockTeachingSubject = new TypeEntity { Id = Guid.NewGuid() }; 

            _mockCrm
                .Setup(mock => mock.GetLookupItems("dfe_teachingsubjectlist"))
                .Returns(new[] { mockTeachingSubject });

            var teachingSubject = new TeachingSubject() {Id = mockTeachingSubject.Id };

            var result = _validator.TestValidate(teachingSubject);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_IdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(teachingSubject => teachingSubject.Id, Guid.NewGuid());
        }
    }
}
