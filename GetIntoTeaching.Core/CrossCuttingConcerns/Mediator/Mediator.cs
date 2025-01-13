using GetIntoTeaching.Core.CrossCuttingConcerns.Mediator.Extensions;
using System.Collections.Concurrent;

namespace GetIntoTeaching.Core.CrossCuttingConcerns.Mediator
{
    public class Mediator : IMediator
    {
        private readonly ServiceFactory _serviceFactory;
        private readonly ConcurrentDictionary<Type, Type> _handlerInfos;

        public Mediator(ServiceFactory serviceFactory, ConcurrentDictionary<Type, Type> handlerInfos)
        {
            _serviceFactory = serviceFactory;
            _handlerInfos = handlerInfos;
        }

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest<TResponse>
            => Send<TRequest, TResponse>(request, CancellationToken.None);

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest<TResponse>
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Type? requestType = request.GetType();

            if (!_handlerInfos.TryGetValue(requestType, out Type? handlerType))
            {
                throw new InvalidOperationException($"No handler found for {requestType.FullName}");
            }

            IHandler<TRequest, TResponse> handler =
                _serviceFactory.GetInstanceWithCast<IHandler<TRequest, TResponse>>(handlerType);

            return handler.Handle(request, cancellationToken);
        }
    }
}