using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using System;
using System.Globalization;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles
{
    public static class CurrentYearProviderTestDouble
    {
        public static ICurrentYearProvider StubFor(DateTimeOffset currentDateTime) =>
            new CurrentYearProviderStub(currentDateTime);

        internal class CurrentYearProviderStub : ICurrentYearProvider
        {
            public CurrentYearProviderStub(DateTimeOffset today) {
                DateTimeToday = today;
            }

            public DateTimeOffset DateTimeToday { get; }

            public int ToYearInt() => Convert.ToInt32(DateTimeToday.Year, CultureInfo.CurrentCulture);

            public int ToYearsAheadInt(int numberOfYearsAhead) =>
                Convert.ToInt32(DateTimeToday.AddYears(
                    numberOfYearsAhead).Year, CultureInfo.CurrentCulture);
        }
    }
}
