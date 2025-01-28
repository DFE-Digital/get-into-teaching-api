using System;

namespace GetIntoTeachingApi.CrossCuttingConcerns.Logging
{
    /// <summary>
    /// Provides the ability to extract a given correlation Id (if available) from the current HTTP context
    /// </summary>
    public interface IHttpContextCorrelationIdProvider
    {
        /// <summary>
        /// Contract for extracting the correlation Id from the current <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>
        /// The Correlation Id (GUID), defaults to Empty if no correlation Id is provisioned.
        /// </returns>
        Guid GetCorrelationId();
    }
}
