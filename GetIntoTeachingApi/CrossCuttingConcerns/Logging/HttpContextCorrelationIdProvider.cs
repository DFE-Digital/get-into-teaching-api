using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System;
using GetIntoTeachingApi.CrossCuttingConcerns.Logging.Common;

namespace GetIntoTeachingApi.CrossCuttingConcerns.Logging
{
    /// <summary>
    /// Provides the ability to extract a given correlation Id (if available) from
    /// the current HTTP context using the provisioned <see cref="IHttpContextAccessor"/> instance.
    /// </summary>
    public sealed class HttpContextCorrelationIdProvider : IHttpContextCorrelationIdProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Provisions the <see cref="IHttpContextAccessor"/> which provides
        /// access to the current <see cref="HttpContext"/>, if one is available.
        /// </summary>
        /// <param name="httpContextAccessor">
        /// The <see cref="IHttpContextAccessor"/> instance used to access the current <see cref="HttpContext"/>.
        /// </param>
        public HttpContextCorrelationIdProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Attempts to extract the correlation Id from the current <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>
        /// The Correlation Id (GUID), defaults to Empty if no correlation Id is provisioned.
        /// </returns>
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
