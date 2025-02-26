using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference;
using Moq;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching.TestDoubles
{
    internal static class DegreeStatusDomainServiceTestDouble
    {
        public static IDegreeStatusDomainService DefaultMock() => Mock.Of<IDegreeStatusDomainService>();

        public static IDegreeStatusDomainService MockFor()//Response<SearchResults<Establishment>> searchResult, string keyword, string collection)
        {
            var degreeStatusDomainServiceMock = new Mock<IDegreeStatusDomainService>();

            //searchServiceMock.Setup(SearchRequest(keyword, collection))
            //    .Returns(Task.FromResult(searchResult))
            //    .Verifiable();

            return degreeStatusDomainServiceMock.Object;
        }
    }
}
