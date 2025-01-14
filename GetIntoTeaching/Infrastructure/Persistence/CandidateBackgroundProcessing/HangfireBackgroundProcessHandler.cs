using GetIntoTeaching.Core.Infrastructure.BackgroundProcessing;
using GetIntoTeaching.Infrastructure.Persistence.CandidateEventProcessing.Common;
using Hangfire;

namespace GetIntoTeaching.Infrastructure.Persistence.CandidateBackgroundProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class HangfireBackgroundProcessHandler : IBackgroundProcessHandler
    {
        private readonly IReadOnlyDictionary<IBackgroundProcessorRequest, IBackgroundProcessor> _backgroundProcessors;
        private readonly IBackgroundJobClient _backgroundJobClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backgroundProcessors"></param>
        /// <param name="backgroundJobClient"></param>
        public HangfireBackgroundProcessHandler(
            IReadOnlyDictionary<IBackgroundProcessorRequest, IBackgroundProcessor> backgroundProcessors,
            IBackgroundJobClient backgroundJobClient)
        {
            _backgroundProcessors = backgroundProcessors;
            _backgroundJobClient = backgroundJobClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processorRequest"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void InvokeProcessor(IBackgroundProcessorRequest processorRequest)
        {
            IBackgroundProcessor? backgroundProcessor =
                _backgroundProcessors[processorRequest] ??
                throw new KeyNotFoundException(
                    "Unable to derive the requested background processor.");

            _backgroundJobClient.Enqueue(() =>
                backgroundProcessor.Process(processorRequest));
        }
    }
}