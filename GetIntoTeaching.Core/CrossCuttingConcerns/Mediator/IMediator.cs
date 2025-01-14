namespace GetIntoTeaching.Core.CrossCuttingConcerns.Mediator
{
    public interface IMediator
    {
        public Task<TResponse> Handle<TRequest, TResponse>(TRequest request)
            where TRequest :
            IRequest<TResponse>;

        public Task<TResponse> Handle<TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken) where TRequest : IRequest<TResponse>;
    }
}