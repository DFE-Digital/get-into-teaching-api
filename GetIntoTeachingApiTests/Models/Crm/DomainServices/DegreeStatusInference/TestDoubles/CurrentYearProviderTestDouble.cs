using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using System;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles
{
    public static class CurrentYearProviderTestDouble
    {
        public static ICurrentYearProvider StubFor(DateTimeOffset currentDateTime) =>
            new CurrentYearProviderStub(currentDateTime);

        internal class CurrentYearProviderStub : ICurrentYearProvider
        {
            public CurrentYearProviderStub(DateTimeOffset today)
            {
                DateTimeToday = today;
            }

            public DateTimeOffset DateTimeToday { get; }

            public DateTimeOffset ToYearsAhead(int numberOfYearsAhead) =>
                DateTimeToday.AddYears(numberOfYearsAhead);
        }
    }
}
