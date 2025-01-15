using GetIntoTeaching.Infrastructure.BackgroundProcessing;
using Hangfire;

namespace GetIntoTeaching.Infrastructure.Persistence.CandidateBackgroundProcessing.Processors
{
    public sealed class UpsertCandidateProcessor : IBackgroundJobProcessor<UpsertCandidateProcessorRequest>
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public UpsertCandidateProcessor(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public Task<BackgroundProcessorResult> Handle(
            UpsertCandidateProcessorRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: we could return a result here to encapsulate any issues!!!!

            _backgroundJobClient.Enqueue(() => RunJob(request));

            return Task.FromResult(new BackgroundProcessorResult());
        }

        public void RunJob(IBackgroundJobProcessorRequest request)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class UpsertCandidateProcessorRequest : IBackgroundJobProcessorRequest
    {
    
    }
}
