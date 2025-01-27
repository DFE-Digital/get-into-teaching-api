using Microsoft.AspNetCore.Http;
using Moq;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles
{
    public static class HttpContextTestDouble
    {
        private static readonly Bogus.Faker faker = new();

        public static Mock<HttpContext> Mock() => new();

        public static HttpContext DefaultMock()
        {
            Mock<HttpContext> httpContextMock = Mock();

            httpContextMock
                .WithSpecificRequestPath(faker.Internet.UrlRootedPath())
                .WithSpecificRequestMethod(faker.Internet.DomainWord())
                .WithSpecificRequestHeader(HeaderDictionaryTestDouble.DefaultMock());

            httpContextMock.SetupGet(context =>
                context.Request.Headers).Returns(HeaderDictionaryTestDouble.DefaultMock());

            return httpContextMock.Object;
        }

        internal static class HeaderDictionaryTestDouble
        {
            public static Mock<IHeaderDictionary> Mock() => new();

            private static readonly string UserAgentHeaderNameKey = "User-Agent";

            public static IHeaderDictionary DefaultMock()
            {
                Mock<IHeaderDictionary> headerDictionaryMock = Mock();

                headerDictionaryMock.SetupGet(dictionary =>
                    dictionary[UserAgentHeaderNameKey]).Returns(faker.Internet.UserAgent());

                return headerDictionaryMock.Object;
            }
        }
    }

    internal static class HttpRequestMockExtensions
    {
        public static Mock<HttpContext> WithSpecificRequestPath(
            this Mock<HttpContext> mockContext, string requestPath)
        {
            mockContext.SetupGet(context => context.Request.Path).Returns(requestPath);
            return mockContext;
        }

        public static Mock<HttpContext> WithSpecificRequestMethod(
            this Mock<HttpContext> mockContext, string requestMethod)
        {
            mockContext.SetupGet(context => context.Request.Method).Returns(requestMethod);
            return mockContext;
        }

        public static Mock<HttpContext> WithSpecificRequestHeader(
            this Mock<HttpContext> mockContext, IHeaderDictionary headerDictionary)
        {
            mockContext.SetupGet(context => context.Request.Headers).Returns(headerDictionary);
            return mockContext;
        }
    }
}
