using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System;

namespace GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers
{
    /// <summary>
    /// 
    /// </summary>
    public class CorrelationIdLogEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpContextCorrelationIdProvider _httpContextCorrelationIdProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public CorrelationIdLogEnricher(
            IHttpContextAccessor httpContextAccessor,
            IHttpContextCorrelationIdProvider httpContextCorrelationIdProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpContextCorrelationIdProvider = httpContextCorrelationIdProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="propertyFactory"></param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is not null)
            {
                logEvent.LogProperty(propertyFactory, PropertyKeys.RequestMethodPropertyNameKey, httpContext.Request.Method);
                logEvent.LogProperty(propertyFactory, PropertyKeys.RequestPathPropertyNameKey, httpContext.Request.Path);
                logEvent.LogProperty(propertyFactory, PropertyKeys.UserAgentPropertyNameKey,
                    httpContext.Request.Headers[PropertyKeys.UserAgentPropertyNameKey]);

                Guid correlationId = _httpContextCorrelationIdProvider.GetCorrelationId();

                if (correlationId != Guid.Empty)
                {
                    logEvent.LogProperty(
                        propertyFactory,
                        PropertyKeys.CorrelationIdNameKey, correlationId);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal static class PropertyKeys
        {
            /// <summary>
            /// 
            /// </summary>
            public static readonly string RequestMethodPropertyNameKey = "RequestMethod";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string RequestPathPropertyNameKey = "RequestPath";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string UserAgentPropertyNameKey = "UserAgent";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string UserAgentHeaderNameKey = "User-Agent";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string CorrelationIdNameKey = "CorrelationId";
        }
    }
}
