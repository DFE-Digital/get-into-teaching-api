using GetIntoTeaching.Infrastructure.Persistence.CandidateEventProcessing.Common;

namespace GetIntoTeaching.Core.Infrastructure.BackgroundProcessing
{
    public interface IBackgroundProcessHandler
    {
        void InvokeProcessor(IBackgroundProcessorRequest processorRequest);
    }
}
