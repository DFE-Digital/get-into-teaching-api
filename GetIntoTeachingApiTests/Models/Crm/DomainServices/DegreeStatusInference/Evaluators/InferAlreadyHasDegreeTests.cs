using FluentAssertions;
using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference;
using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators;
using GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    public sealed class InferAlreadyHasDegreeTests
    {
        [Fact]
        public void CanEvaluate_WithGraduationYearBehindCurrentYear_ReturnsTrue()
        {
            // arrange
            InferAlreadyHasDegree inferAlreadyHasDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2024, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            bool canEvaluate = inferAlreadyHasDegree.CanEvaluate(degreeStatusInferenceRequest);

            // assert
            Assert.True(canEvaluate);
        }

        [Fact]
        public void CanEvaluate_WithGraduationYearAheadOfCurrentYear_ReturnsFalse()
        {
            // arrange
            InferAlreadyHasDegree inferAlreadyHasDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2026, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            bool canEvaluate = inferAlreadyHasDegree.CanEvaluate(degreeStatusInferenceRequest);

            // assert
            Assert.False(canEvaluate);
        }

        [Fact]
        public void Evaluate_WithGraduationYearBehindCurrentYear_ReturnsHasDegreeStatus()
        {
            // arrange
            InferAlreadyHasDegree inferAlreadyHasDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2024, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            DegreeStatus degreeStatus = inferAlreadyHasDegree.Evaluate(degreeStatusInferenceRequest);

            // assert
            Assert.Equal(DegreeStatus.HasDegree, degreeStatus);
        }

        [Fact]
        public void Evaluate_WithGraduationYearAheadOfCurrentYear_ThrowsArgumentOutOfRangeException()
        {
            // arrange
            InferAlreadyHasDegree inferAlreadyHasDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2026, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act, assert
            Action failedAction =
                () => inferAlreadyHasDegree.Evaluate(degreeStatusInferenceRequest);

            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(failedAction);

            exception.Message.Should().Be("Year must be the current year. (Parameter 'evaluationRequest')");
        }
    }
}
