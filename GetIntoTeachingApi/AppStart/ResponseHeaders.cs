using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace GetIntoTeachingApi.AppStart
{
    public static class ResponseHeaders
    {
        public static void SetupSecureHeaders(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("X-Frame-Options", "deny");
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
                await next();
            });
        }
    }
}