using System;
using Hangfire.Server;

namespace GetIntoTeachingApi.Adapters
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformContextAdapter : IPerformContextAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid JobCorrelationContext{ get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public int GetRetryCount(PerformContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return context.GetJobParameter<int>("RetryCount");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public DateTime GetJobCreatedAt(PerformContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return context.BackgroundJob.CreatedAt;
        }
    }
}
