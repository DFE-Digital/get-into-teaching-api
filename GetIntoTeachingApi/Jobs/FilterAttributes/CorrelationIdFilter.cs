using GetIntoTeachingApi.CrossCuttingConcerns.Logging;
using Hangfire.Client;
using System;

namespace GetIntoTeachingApi.Jobs.FilterAttributes
{
    /// <summary>
    /// Provides the attribute filter implementation for assigning known
    /// http context related correlation Id's to the associated job context.
    /// </summary>
    public class CorrelationIdFilter : IClientFilter
    {
        private readonly IHttpContextCorrelationIdProvider _httpContextCorrelationIdProvider;

        /// <summary>
        /// THe job parameter name key to assign to the correlation Id.
        /// </summary>
        public static readonly string CorrelationIdKey = "CorrelationId";

        /// <summary>
        /// The <see cref="IHttpContextCorrelationIdProvider"/> provides the ability to
        /// extract a given correlation Id (if available) from the current HTTP context.
        /// </summary>
        /// <param name="httpContextCorrelationIdProvider"></param>
        public CorrelationIdFilter(IHttpContextCorrelationIdProvider httpContextCorrelationIdProvider)
        {
            _httpContextCorrelationIdProvider = httpContextCorrelationIdProvider;
        }

        /// <summary>
        /// Called before the creation of the job, and attempts to assign a correlation
        /// Id (if one is configured from <see cref="IHttpContextCorrelationIdProvider">)
        /// to the <see cref="CreateContext"/> instance.
        /// </summary>
        /// <param name="context">
        /// The <see cref="CreatingContext"/> provides the context for the
        /// Hangfire.Client.IClientFilter.OnCreating(Hangfire.Client.CreatingContext)
        /// method of the Hangfire.Client.IClientFilter interface.
        /// </param>
        public void OnCreating(CreatingContext context)
        {
            Guid correlationId = _httpContextCorrelationIdProvider.GetCorrelationId();

            if (correlationId != Guid.Empty)
            {
                context.SetJobParameter(CorrelationIdKey, $"CID-{correlationId}");
            }
        }

        public void OnCreated(CreatedContext context)
        {
            // allow default behavior.
        }
    }
}
