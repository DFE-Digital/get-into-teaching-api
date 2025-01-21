using System;

namespace GetIntoTeachingApi.CrossCuttingConcerns.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHttpContextCorrelationIdProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Guid GetCorrelationId();
    }
}
