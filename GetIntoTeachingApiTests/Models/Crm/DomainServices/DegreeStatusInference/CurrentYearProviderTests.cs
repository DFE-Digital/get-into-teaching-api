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
            int year =
                currentYearProvider.ToYearInt();

            // assert
            Assert.IsType<int>(year);
            Assert.Equal(DateTimeOffset.UtcNow.Year, year);
        }

        [Fact]
        public void ToYearsAheadInt_EnsureAConfiguredIntYearAheadIsReturned()
        {
            // arrange
            CurrentYearProvider currentYearProvider = new();

            // act
            int year =
                currentYearProvider.ToYearsAheadInt(3);

            // assert
            Assert.IsType<int>(year);
            Assert.Equal(DateTimeOffset.UtcNow.Year + 3, year);
        }
    }
}
