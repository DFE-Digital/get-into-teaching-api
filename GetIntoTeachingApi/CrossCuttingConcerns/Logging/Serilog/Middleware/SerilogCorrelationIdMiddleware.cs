using GetIntoTeachingApi.CrossCuttingConcerns.Logging.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.Middleware
{
    /// <summary>
    /// Middleware used to access the given application's request pipeline, and add a correlation Id
    /// (GUID) to the <see cref="HttpContext"/> feature collection <see cref="IFeatureCollection"/>
    /// to allow extraction at a point further downstream, i.e. via a <see cref="IHttpContextCorrelationIdProvider"/>.
    /// </summary>
    public sealed class SerilogCorrelationIdMiddleware : IMiddleware
    {
        /// <summary>
        /// Request handling method which provisions a new correlation Id (GUID) to the current request.
        /// </summary>
        /// <param name="context">
        /// The <see cref="HttpContext"/> instance used to access the current request context.
        /// </param>
        /// <param name="next">
        /// The <see cref="RequestDelegate"/> instance used to invoke the next middleware in the pipeline.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> instance used to represent the asynchronous operation.
        /// </returns>
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Guid correleationId = Guid.NewGuid();

            IHttpActivityFeature activityFeature =
                context.Features.GetRequiredFeature<IHttpActivityFeature>();
            Activity activity = activityFeature.Activity;
            activity.AddTag(CorrelationPropertyKeys.PerRequestCorrelationIdPropertyNameKey, correleationId);

            return next(context);
        }
    }
}
