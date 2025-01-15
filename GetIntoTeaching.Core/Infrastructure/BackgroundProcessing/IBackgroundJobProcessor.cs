using GetIntoTeaching.Core.CrossCuttingConcerns.Mediator;

namespace GetIntoTeaching.Infrastructure.BackgroundProcessing
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
