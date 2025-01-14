using GetIntoTeaching.Core.CrossCuttingConcerns.Mediator;
using GetIntoTeaching.Infrastructure.Persistence.CandidateBackgroundProcessing.Processors;

namespace GetIntoTeaching.Infrastructure.Persistence.CandidateEventProcessing.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBackgroundJobProcessor<TProcessorRequest> :
        IHandler<TProcessorRequest, BackgroundProcessorResult>
            where TProcessorRequest : IBackgroundJobProcessorRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        void RunJob(IBackgroundJobProcessorRequest request);
    }
}
