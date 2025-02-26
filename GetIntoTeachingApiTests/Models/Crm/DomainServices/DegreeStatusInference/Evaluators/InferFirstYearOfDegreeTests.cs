using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators;
using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference;
using GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles;
using System;
using Xunit;
using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    public sealed class InferFirstYearOfDegreeTests
    {
        [Fact]
        public void CanEvaluate_WithGraduationYearThreeYearsAheadOfCurrentYear_ReturnsTrue()
        {
            // arrange
            InferFirstYearOfDegree inferFirstYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2028, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            bool canEvaluate = inferFirstYearOfDegree.CanEvaluate(degreeStatusInferenceRequest);

            // assert
            Assert.True(canEvaluate);
        }

        [Fact]
        public void CanEvaluate_WithGraduationYearMoreThanThreeYearsAheadOfCurrentYear_ReturnsTrue()
        {
            // arrange
            InferFirstYearOfDegree inferFirstYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2030, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            bool canEvaluate = inferFirstYearOfDegree.CanEvaluate(degreeStatusInferenceRequest);

            // assert
            Assert.True(canEvaluate);
        }

        [Fact]
        public void CanEvaluate_WithGraduationYearNotThreeYearsAheadOfCurrentYear_ReturnsFalse()
        {
            // arrange
            InferFirstYearOfDegree inferFirstYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2026, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            bool canEvaluate = inferFirstYearOfDegree.CanEvaluate(degreeStatusInferenceRequest);

            // assert
            Assert.False(canEvaluate);
        }

        [Fact]
        public void Evaluate_WithGraduationYearThreeYearsAheadOfCurrentYear_ReturnsSecondYearStatus()
        {
            // arrange
            InferFirstYearOfDegree inferFirstYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2028, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            DegreeStatus degreeStatus = inferFirstYearOfDegree.Evaluate(degreeStatusInferenceRequest);

            // assert
            Assert.Equal(DegreeStatus.FirstYear, degreeStatus);
        }

        [Fact]
        public void Evaluate_WithGraduationYearMoreThanThreeYearsAheadOfCurrentYear_ReturnsSecondYearStatus()
        {
            // arrange
            InferFirstYearOfDegree inferFirstYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2030, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            DegreeStatus degreeStatus = inferFirstYearOfDegree.Evaluate(degreeStatusInferenceRequest);

            // assert
            Assert.Equal(DegreeStatus.FirstYear, degreeStatus);
        }

        [Fact]
        public void Evaluate_WithGraduationYearNotThreeYearsAheadOfCurrentYear_ThrowsArgumentOutOfRangeException()
        {
            // arrange
            InferFirstYearOfDegree inferFirstYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2027, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act, assert
            Action failedAction =
                () => inferFirstYearOfDegree.Evaluate(degreeStatusInferenceRequest);

            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(failedAction);

            exception.Message.Should().Be("Year must be the current year. (Parameter 'evaluationRequest')");
        }
    }
}
