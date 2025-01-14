using GetIntoTeaching.Core.CrossCuttingConcerns.Mediator;
using GetIntoTeaching.Infrastructure.Persistence.CandidateBackgroundProcessing.Processors;

namespace GetIntoTeaching.Infrastructure.Persistence.CandidateEventProcessing.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBackgroundJobProcessorRequest :
        IRequest<BackgroundProcessorResult>
    {
    }
}
