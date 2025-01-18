using GetIntoTeaching.Infrastructure.BackgroundProcessing;
using Hangfire;

namespace GetIntoTeaching.Infrastructure.Persistence.CandidateBackgroundProcessing.Processors
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UpsertCandidateProcessor : IBackgroundJobProcessor<UpsertCandidateProcessorRequest>
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backgroundJobClient"></param>
        public UpsertCandidateProcessor(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<BackgroundProcessorResult> Handle(
            UpsertCandidateProcessorRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: we could return a result here to encapsulate any issues!!!!

            _backgroundJobClient.Enqueue(() => RunJob(request));

            return Task.FromResult(new BackgroundProcessorResult());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void RunJob(IBackgroundJobProcessorRequest request)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class UpsertCandidateProcessorRequest : IBackgroundJobProcessorRequest
    {
    
    }
}
