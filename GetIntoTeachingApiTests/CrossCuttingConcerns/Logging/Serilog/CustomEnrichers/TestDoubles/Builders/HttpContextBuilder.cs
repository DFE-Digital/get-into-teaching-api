using Microsoft.AspNetCore.Http;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles.Builders
{
    public sealed class HttpContextBuilder
    {
        private readonly HttpContext _httpContext;

        private HttpContextBuilder()
        {
            _httpContext = new DefaultHttpContext();
        }

        public HttpContext Build() => _httpContext;
    }
}
