using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators;
using GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    public sealed class InferFinalYearOfDegreeTests
    {
        [Fact]
        public void CanEvaluate_WithGraduationYearProvidedWithinCurrentAcademicYear_ReturnsTrue()
        {
            // arrange
            InferFinalYearOfDegree inferFinalYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 03, 27));

            GraduationYear graduationYear = new(year: 2025, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            DegreeStatus degreeStatus = inferFinalYearOfDegree.Evaluate(degreeStatusInferenceRequest);

            // assert
            Assert.Equal(DegreeStatus.FinalYear, degreeStatus);
        }

        [Fact]
        public void CanEvaluate_WithGraduationYearProvidedNotWithinCurrentAcademicYear_ReturnsFalse()
        {
            // arrange
            InferFinalYearOfDegree inferFinalYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2026, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            bool canEvaluate = inferFinalYearOfDegree.CanEvaluate(degreeStatusInferenceRequest);

            // assert
            Assert.False(canEvaluate);
        }

        [Fact]
        public void Evaluate_WithGraduationYearProvidedWithinCurrentAcademicYear_ReturnsFinalYearStatus()
        {
            // arrange
            InferFinalYearOfDegree inferFinalYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 03, 27));

            GraduationYear graduationYear = new(year: 2025, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            DegreeStatus degreeStatus = inferFinalYearOfDegree.Evaluate(degreeStatusInferenceRequest);

            // assert
            Assert.Equal(DegreeStatus.FinalYear, degreeStatus);
        }

        [Fact]
        public void Evaluate_WithGraduationYearProvidedNotWithinCurrentAcademicYear_ThrowsArgumentOutOfRangeException()
        {
            // arrange
            InferFinalYearOfDegree inferFinalYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2026, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act, assert
            Action failedAction =
                () => inferFinalYearOfDegree.Evaluate(degreeStatusInferenceRequest);

            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(failedAction);

            exception.Message.Should().Be("Graduation year provided must fall within the current academic year. (Parameter 'evaluationRequest')");
        }
    }
}
