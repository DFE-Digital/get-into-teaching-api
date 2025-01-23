using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System;

namespace GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers
{
    /// <summary>
    /// Log event enricher allows associated events to have a
    /// correlation Id (GUID) to be assigned to a named property
    /// to allow to allow various log events to be aggregated across a single request.
    /// </summary>
    public class CorrelationIdLogEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpContextCorrelationIdProvider _httpContextCorrelationIdProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor">
        /// 
        /// </param>
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
                logEvent
                    .LogProperty(propertyFactory,
                        LogPropertyKeys.RequestMethodPropertyNameKey, httpContext.Request.Method)
                    .LogProperty(propertyFactory,
                        LogPropertyKeys.RequestPathPropertyNameKey, httpContext.Request.Path)
                    .LogProperty(propertyFactory,
                        LogPropertyKeys.UserAgentPropertyNameKey,
                        httpContext.Request.Headers[LogPropertyKeys.UserAgentHeaderNameKey]);

                Guid correlationId = _httpContextCorrelationIdProvider.GetCorrelationId();

                if (correlationId != Guid.Empty)
                {
                    logEvent.LogProperty(propertyFactory,
                        LogPropertyKeys.CorrelationIdNameKey, correlationId);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal readonly struct LogPropertyKeys
        {
            public static readonly string RequestMethodPropertyNameKey = "RequestMethod";
            public static readonly string RequestPathPropertyNameKey = "RequestPath";
            public static readonly string UserAgentPropertyNameKey = "UserAgent";
            public static readonly string UserAgentHeaderNameKey = "User-Agent";
            public static readonly string CorrelationIdNameKey = "CorrelationId";
        }
    }
}
