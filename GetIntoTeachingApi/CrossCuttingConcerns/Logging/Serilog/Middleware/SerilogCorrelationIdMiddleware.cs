using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SerilogCorrelationIdMiddleware : IMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        private const string PerRequestCorrelationIdPropertyNameKey = "PerRequestCorrelationId";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async  Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Guid correleationId = Guid.NewGuid();

            var activityFeature = context.Features.GetRequiredFeature<IHttpActivityFeature>();
            var activity = activityFeature.Activity;
            activity.AddTag(PerRequestCorrelationIdPropertyNameKey, correleationId);

            await next(context);
        }
    }
}
