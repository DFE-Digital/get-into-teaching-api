using Microsoft.AspNetCore.Http;
using Moq;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.Shared.TestDoubles
{
    public static class HttpContextAccessorTestDouble
    {
        public static Mock<IHttpContextAccessor> Mock() => new();

        public static IHttpContextAccessor DefaultMock() => MockFor(HttpContextTestDouble.DefaultMock());

        public static IHttpContextAccessor MockFor(HttpContext context)
        {
            var httpContextAccessor = Mock();

            httpContextAccessor
                .SetupGet(accessor => accessor.HttpContext).Returns(context);

            return httpContextAccessor.Object;
        }
    }
}
