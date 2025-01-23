using Microsoft.AspNetCore.Http;
using Moq;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles
{
    public static class HttpContextTestDouble
    {
        public static Mock<HttpContext> Mock() => new();

        public static HttpContext MockFor()
        {
            Mock<HttpContext> httpContextMock = Mock();

            return httpContextMock.Object;
        }
    }
}
