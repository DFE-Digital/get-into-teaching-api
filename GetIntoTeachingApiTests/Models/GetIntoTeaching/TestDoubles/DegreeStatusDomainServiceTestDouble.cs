using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using Moq;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching.TestDoubles
{
    internal static class DegreeStatusDomainServiceTestDouble
    {
        public static IDegreeStatusDomainService DefaultMock() => Mock.Of<IDegreeStatusDomainService>();

        public static IDegreeStatusDomainService DefaultMockObject() => new Mock<IDegreeStatusDomainService>().Object;
    }
}
