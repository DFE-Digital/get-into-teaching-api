using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System;
using GetIntoTeachingApi.CrossCuttingConcerns.Logging.Common;

namespace GetIntoTeachingApi.CrossCuttingConcerns.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class HttpContextCorrelationIdProvider : IHttpContextCorrelationIdProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public HttpContextCorrelationIdProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Guid GetCorrelationId()
        {
            Guid correlationId = Guid.Empty;
            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is not null)
            {
                IHttpActivityFeature httpActivityFeature =
                    httpContext.Features.GetRequiredFeature<IHttpActivityFeature>();

                Activity activity = httpActivityFeature.Activity;

                object httpRequestCorrelationId =
                    activity.GetTagItem(CorrelationPropertyKeys.PerRequestCorrelationIdPropertyNameKey);

                if (httpRequestCorrelationId is not null)
                {
                    correlationId = (Guid)httpRequestCorrelationId;
                }
            }

            return correlationId;
        }
    }
}
