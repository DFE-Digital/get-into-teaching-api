using GetIntoTeachingApi.Middleware;
using Microsoft.AspNetCore.Builder;

namespace GetIntoTeachingApi.Utils
{
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
