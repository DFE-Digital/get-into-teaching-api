using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using Moq;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching.TestDoubles
{
    internal static class CurrentYearProviderTestDouble
    {
        public static ICurrentYearProvider DefaultMock() => Mock.Of<ICurrentYearProvider>();

        public static ICurrentYearProvider DefaultMockObject() => new Mock<ICurrentYearProvider>().Object;
    }
}
