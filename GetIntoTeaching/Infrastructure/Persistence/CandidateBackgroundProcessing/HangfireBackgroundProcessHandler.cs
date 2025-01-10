using GetIntoTeaching.Core.Infrastructure.BackgroundProcessing;
using GetIntoTeaching.Infrastructure.Persistence.CandidateEventProcessing.Common;

namespace GetIntoTeaching.Infrastructure.Persistence.CandidateBackgroundProcessing
{
    public sealed class HangfireBackgroundProcessHandler : IBackgroundProcessHandler
    {
        private readonly IReadOnlyDictionary<IBackgroundProcessorRequest, IBackgroundProcessor> _backgroundProcessors;

        public HangfireBackgroundProcessHandler(IReadOnlyDictionary<IBackgroundProcessorRequest, IBackgroundProcessor> backgroundProcessors)
        {
            _backgroundProcessors = backgroundProcessors;
        }

        public TResult InvokeProcessor<TResult>()
        {
            throw new NotImplementedException();
        }
    }
}