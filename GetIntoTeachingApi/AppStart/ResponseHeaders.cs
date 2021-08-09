using Microsoft.AspNetCore.Builder;

namespace GetIntoTeachingApi.AppStart
{
    public static class ResponseHeaders
    {
        public static void SetupSecureHeaders(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "deny");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
                await next();
            });
        }
    }
}