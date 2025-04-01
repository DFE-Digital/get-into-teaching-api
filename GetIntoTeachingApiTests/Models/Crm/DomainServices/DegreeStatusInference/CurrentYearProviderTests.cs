using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference
{
    public sealed class CurrentYearProviderTests
    {
        [Fact]
        public void DateTimeToday_EnsureAConfiguredDateTimeOffsetIsReturned()
        {
            // arrange
            CurrentYearProvider currentYearProvider = new();

            // act
            DateTimeOffset dateTimeToday =
                currentYearProvider.DateTimeToday;

            // assert
            Assert.NotEqual(DateTimeOffset.MinValue, dateTimeToday);
        }

        [Fact]
        public void ToYearInt_EnsureAConfiguredIntYearIsReturned()
        {
            // arrange
            CurrentYearProvider currentYearProvider = new();

            // act
            DateTimeOffset academicYear =
                currentYearProvider.DateTimeToday;

            // assert
            Assert.IsType<DateTimeOffset>(academicYear);
            Assert.Equal(DateTimeOffset.UtcNow.Year, academicYear.Year);
            Assert.Equal(DateTimeOffset.UtcNow.Month, academicYear.Month);
            Assert.Equal(DateTimeOffset.UtcNow.Day, academicYear.Day);
        }

        [Fact]
        public void ToYearsAheadInt_EnsureAConfiguredIntYearAheadIsReturned()
        {
            // arrange
            CurrentYearProvider currentYearProvider = new();

            // act
            DateTimeOffset academicYear =
                currentYearProvider.ToYearsAhead(3);

            // assert
            Assert.IsType<DateTimeOffset>(academicYear);
            Assert.Equal(DateTimeOffset.UtcNow.AddYears(3).Year, academicYear.Year);
            Assert.Equal(DateTimeOffset.UtcNow.AddYears(3).Month, academicYear.Month);
            Assert.Equal(DateTimeOffset.UtcNow.AddYears(3).Day, academicYear.Day);
        }
    }
}
