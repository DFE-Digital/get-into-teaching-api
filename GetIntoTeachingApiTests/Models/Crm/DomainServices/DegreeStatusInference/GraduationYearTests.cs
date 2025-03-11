using FluentAssertions;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference
{
    public sealed class GraduationYearTests
    {
        private readonly GraduationYear _graduationYear;
        private readonly ICurrentYearProvider _currentYearProvider;

        public GraduationYearTests()
        {
            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2021, 01, 01));

            _currentYearProvider = currentYearProvider;

            _graduationYear = new GraduationYear(2021, currentYearProvider);
        }

        [Fact]
        public void Ctor_withYearParamAboveMaxAllowedValue_throwsExpectedException()
        {
            // act
            Action action = () =>
                new GraduationYear(year: 2062, currentYearProvider: _currentYearProvider);

            // result
            action.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Year must be less than 2061 (Parameter 'year')");
        }

        [Fact]
        public void GetHashcode_EnsureYearValueReturnsExpectedHashcodeForEquality()
        {
            // arrange

            // act
            int hashcode = _graduationYear.GetHashCode();

            // assert
            Assert.Equal(2021.GetHashCode(), hashcode);
        }

        [Fact]
        public void Equals_NullEqualityObjectPassed_ReturnsFalse()
        {
            // act
            bool result = _graduationYear.Equals(null!);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithGraduationYearEqualityObjectPassed_ReturnsTrue()
        {
            // act
            bool result = _graduationYear.Equals(new GraduationYear(2021, _currentYearProvider));

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithGraduationYearNonEqualityObjectPassed_ReturnsFalse()
        {
            // act
            bool result = _graduationYear.Equals(new GraduationYear(2022, _currentYearProvider));

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithGraduationYearAsIntEqualityObjectPassed_ReturnsTrue()
        {
            // act
            bool result = _graduationYear.Equals(2021);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithGraduationYearAsIntNonEqualityObjectPassed_ReturnsFalse()
        {
            // act
            bool result = _graduationYear.Equals(2022);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithGraduationYearAsDateTimeEqualityObjectPassed_ReturnsTrue()
        {
            // act
            bool result = _graduationYear.Equals(new DateTime(2021, 01, 01));

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithGraduationYearAsDateTimeNonEqualityObjectPassed_ReturnsFalse()
        {
            // act
            bool result = _graduationYear.Equals(new DateTime(2022, 01, 01));

            // assert
            Assert.False(result);
        }
    }
}
