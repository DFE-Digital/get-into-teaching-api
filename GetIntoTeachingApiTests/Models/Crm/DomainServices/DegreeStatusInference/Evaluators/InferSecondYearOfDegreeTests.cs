using GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles;
using System;
using Xunit;
using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    public sealed class InferSecondYearOfDegreeTests
    {
        [Fact]
        public void CanEvaluate_WithGraduationYearTwoYearsAheadOfCurrentYear_ReturnsTrue()
        {
            // arrange
            InferSecondYearOfDegree inferSecondYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2027, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            bool canEvaluate = inferSecondYearOfDegree.CanEvaluate(degreeStatusInferenceRequest);

            // assert
            Assert.True(canEvaluate);
        }

        [Fact]
        public void CanEvaluate_WithGraduationYearNotTwoYearsAheadOfCurrentYear_ReturnsFalse()
        {
            // arrange
            InferSecondYearOfDegree inferSecondYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2029, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            bool canEvaluate = inferSecondYearOfDegree.CanEvaluate(degreeStatusInferenceRequest);

            // assert
            Assert.False(canEvaluate);
        }

        [Fact]
        public void Evaluate_WithGraduationYearTwoYearsAheadOfCurrentYear_ReturnsSecondYearStatus()
        {
            // arrange
            InferSecondYearOfDegree inferSecondYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2027, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act
            DegreeStatus degreeStatus = inferSecondYearOfDegree.Evaluate(degreeStatusInferenceRequest);

            // assert
            Assert.Equal(DegreeStatus.SecondYear, degreeStatus);
        }

        [Fact]
        public void Evaluate_WithGraduationYearNotOneYearAheadOfCurrentYear_ThrowsArgumentOutOfRangeException()
        {
            // arrange
            InferSecondYearOfDegree inferSecondYearOfDegree = new();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01));

            GraduationYear graduationYear = new(year: 2029, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);

            // act, assert
            Action failedAction =
                () => inferSecondYearOfDegree.Evaluate(degreeStatusInferenceRequest);

            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(failedAction);

            exception.Message.Should().Be("Year must be 2 years from 2025. (Parameter 'evaluationRequest')");
        }
    }
}
