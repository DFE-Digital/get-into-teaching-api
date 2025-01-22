using GetIntoTeachingApi.CrossCuttingConcerns.Logging;
using Hangfire.Client;
using System;

namespace GetIntoTeachingApi.Jobs.FilterAttributes
{
    /// <summary>
    /// Provides the attribute filter implementation for assigning known
    /// http context related correlation Id's to the associated job context.
    /// </summary>
    public sealed class CorrelationIdFilter : IClientFilter
    {
        private readonly IHttpContextCorrelationIdProvider _httpContextCorrelationIdProvider;

        /// <summary>
        /// 
        /// </summary>
        public static readonly string CorrelationIdKey = "CorrelationId";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextCorrelationIdProvider"></param>
        public CorrelationIdFilter(IHttpContextCorrelationIdProvider httpContextCorrelationIdProvider)
        {
            _httpContextCorrelationIdProvider = httpContextCorrelationIdProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnCreating(CreatingContext context)
        {
            Guid correlationId = _httpContextCorrelationIdProvider.GetCorrelationId();

            if (correlationId != Guid.Empty)
            {
                context.SetJobParameter(CorrelationIdKey, correlationId);
            }
        }

        public void OnCreated(CreatedContext context)
        {
            // allow default behavior.
        }
    }
}
