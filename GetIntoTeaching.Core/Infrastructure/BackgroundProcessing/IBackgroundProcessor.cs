namespace GetIntoTeaching.Infrastructure.Persistence.CandidateEventProcessing.Common
{
    public interface IBackgroundProcessor<in TRequest, TResponse>
        where TRequest : IBackgroundProcessorRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
    }
}
