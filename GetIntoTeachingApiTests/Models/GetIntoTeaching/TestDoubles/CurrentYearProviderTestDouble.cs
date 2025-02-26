using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference;
using Moq;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching.TestDoubles
{
    internal static class CurrentYearProviderTestDouble
    {
        public static ICurrentYearProvider DefaultMock() => Mock.Of<ICurrentYearProvider>();

        public static ICurrentYearProvider MockFor()//Response<SearchResults<Establishment>> searchResult, string keyword, string collection)
        {
            var currentYearProviderMock = new Mock<ICurrentYearProvider>();

            //searchServiceMock.Setup(SearchRequest(keyword, collection))
            //    .Returns(Task.FromResult(searchResult))
            //    .Verifiable();

            return currentYearProviderMock.Object;
        }
    }
}
