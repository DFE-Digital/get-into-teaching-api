using GetIntoTeaching.Core.Infrastructure.BackgroundProcessing;
using GetIntoTeaching.Infrastructure.Persistence.CandidateEventProcessing.Common;

namespace GetIntoTeaching.Infrastructure.Persistence.CandidateBackgroundProcessing
{
    public sealed class HangfireBackgroundProcessHandler : IBackgroundProcessHandler
    {
        private readonly IReadOnlyDictionary<IBackgroundProcessorRequest, IBackgroundProcessor> _backgroundProcessors;

        public HangfireBackgroundProcessHandler(
            IReadOnlyDictionary<IBackgroundProcessorRequest, IBackgroundProcessor> backgroundProcessors)
        {
            _backgroundProcessors = backgroundProcessors;
        }

        public void InvokeProcessor(IBackgroundProcessorRequest processorRequest)
        {
            IBackgroundProcessor? backgroundProcessor =
                _backgroundProcessors[processorRequest] ??
                throw new KeyNotFoundException(
                    "Unable to derive the requested background processor.");

            backgroundProcessor.Process(processorRequest);
        }
    }
}