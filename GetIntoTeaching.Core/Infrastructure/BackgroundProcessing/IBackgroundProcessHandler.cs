using GetIntoTeaching.Infrastructure.Persistence.CandidateEventProcessing.Common;

namespace GetIntoTeaching.Core.Infrastructure.BackgroundProcessing
{
    public interface IBackgroundProcessMediator
    {
        public Task<TResponse> InvokeProcessor<TRequest, TResponse>(TRequest request)
            where TRequest : IBackgroundProcessorRequest<TResponse>;
        //public Task<TResponse> InvokeProcessor<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        //    where TRequest : IRequest<TResponse>;
    }
}
