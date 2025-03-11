using GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles;
using System;
using Xunit;
using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    public sealed class InferFinalYearOfDegreeTests
    {
        [Fact]
        public void CanEvaluate_WithGraduationYearOneYearAheadOfCurrentYear_ReturnsTrue()
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
            Assert.True(canEvaluate);
        }

        [Fact]
        public void CanEvaluate_WithGraduationYearNotOneYearAheadOfCurrentYear_ReturnsFalse()
        {
            // arrange
            InferFinalYearOfDegree inferFinalYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2027, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            bool canEvaluate = inferFinalYearOfDegree.CanEvaluate(degreeStatusInferenceRequest);

            // assert
            Assert.False(canEvaluate);
        }

        [Fact]
        public void Evaluate_WithGraduationYearOneYearAheadOfCurrentYear_ReturnsFinalYearStatus()
        {
            // arrange
            InferFinalYearOfDegree inferFinalYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2026, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            DegreeStatus degreeStatus = inferFinalYearOfDegree.Evaluate(degreeStatusInferenceRequest);

            // assert
            Assert.Equal(DegreeStatus.FinalYear, degreeStatus);
        }

        [Fact]
        public void Evaluate_WithGraduationYearNotOneYearAheadOfCurrentYear_ThrowsArgumentOutOfRangeException()
        {
            // arrange
            InferFinalYearOfDegree inferFinalYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2027, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act, assert
            Action failedAction =
                () => inferFinalYearOfDegree.Evaluate(degreeStatusInferenceRequest);

            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(failedAction);

            exception.Message.Should().Be("Year must be 1 years from 2025. (Parameter 'evaluationRequest')");
        }
    }
}
