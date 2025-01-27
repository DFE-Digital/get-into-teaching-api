using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System;

namespace GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers
{
    /// <summary>
    /// Log event enricher allows associated events to have a
    /// correlation Id (GUID) to be assigned to a named property allowing
    /// various log events to be aggregated across a single request.
    /// </summary>
    public class CorrelationIdLogEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpContextCorrelationIdProvider _httpContextCorrelationIdProvider;

        /// <summary>
        /// Initialisation requires a <see cref="IHttpContextAccessor"/> which provides
        /// access to the current <see cref="HttpContext"/>, if one is available; and a
        /// <see cref="IHttpContextCorrelationIdProvider"/> which provides a correlation 
        /// Id (GUID) used to aggregates log events across a single request.
        /// </summary>
        /// <param name="httpContextAccessor">
        /// Instance of <see cref="IHttpContextAccessor"/> which provides
        /// access to the current <see cref="HttpContext"/>, if available.
        /// </param>
        /// /// <param name="httpContextCorrelationIdProvider">
        /// Instance of <see cref="IHttpContextCorrelationIdProvider"/> which provides
        /// a correlation Id (GUID) used to aggregates log events across a single request.
        /// </param>
        public CorrelationIdLogEnricher(
            IHttpContextAccessor httpContextAccessor,
            IHttpContextCorrelationIdProvider httpContextCorrelationIdProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpContextCorrelationIdProvider = httpContextCorrelationIdProvider;
        }

        /// <summary>
        /// Call to enrich decorates each 'enriched' log event with additional
        /// properties, including the correlation Id derived from the current request.
        /// </summary>
        /// <param name="logEvent">
        /// The contextual <see cref="LogEvent"/> on which additional
        /// properties will be applied/enriched. 
        /// </param>
        /// <param name="propertyFactory">
        /// The <see cref="ILogEventPropertyFactory"/> factory object used to create
        /// log event properties from regular .NET objects, applying policies as required.
        /// </param>
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
        /// Aggregation of related log property keys used to define a given log property name.
        /// </summary>
        internal readonly struct LogPropertyKeys
        {
            /// <summary>
            /// httpContext.Request.Method property name key.
            /// </summary>
            public static readonly string RequestMethodPropertyNameKey = "RequestMethod";
            /// <summary>
            /// httpContext.Request.Path property name key.
            /// </summary>
            public static readonly string RequestPathPropertyNameKey = "RequestPath";
            /// <summary>
            /// httpContext.Request.Headers user agent property name key.
            /// </summary>
            public static readonly string UserAgentPropertyNameKey = "UserAgent";
            /// <summary>
            /// httpContext.Request.Headers user agent header index key.
            /// </summary>
            public static readonly string UserAgentHeaderNameKey = "User-Agent";
            /// <summary>
            /// The specific request correlation Id (GUID).
            /// </summary>
            public static readonly string CorrelationIdNameKey = "CorrelationId";
        }
    }
}
