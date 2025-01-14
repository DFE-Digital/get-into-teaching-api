using GetIntoTeaching.Core.CrossCuttingConcerns.Mediator;

namespace GetIntoTeaching.Infrastructure.Persistence.CandidateBackgroundProcessing.Processors
{
    public sealed class UpsertCandidateProcessor : IHandler<UpsertCandidateProcessorRequest, bool>
    {
        public Task<bool> Handle(UpsertCandidateProcessorRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class UpsertCandidateProcessorRequest : IRequest<bool>
    { }
}
