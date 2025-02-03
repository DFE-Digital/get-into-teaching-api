using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference;
using System;
using System.Globalization;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles
{
    public static class CurrentYearProviderTestDouble
    {
        public static ICurrentYearProvider StubFor(DateTime currentDateTime) =>
            new CurrentYearProviderStub(currentDateTime);

        internal class CurrentYearProviderStub : ICurrentYearProvider
        {
            public CurrentYearProviderStub(DateTime today) {
                DateTimeToday = today;
            }

            public DateTime DateTimeToday { get; }

            public int ToYearInt() => Convert.ToInt32(DateTimeToday.Year, CultureInfo.CurrentCulture);

            public int ToYearsAheadInt(int numberOfYearsAhead) =>
                Convert.ToInt32(DateTimeToday.AddYears(
                    numberOfYearsAhead).Year, CultureInfo.CurrentCulture);
        }
    }
}
