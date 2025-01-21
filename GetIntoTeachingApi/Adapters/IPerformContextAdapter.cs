using System;
using Hangfire.Server;

namespace GetIntoTeachingApi.Adapters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPerformContextAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        Guid JobCorrelationContext { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        int GetRetryCount(PerformContext context);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        DateTime GetJobCreatedAt(PerformContext context);
    }
}
