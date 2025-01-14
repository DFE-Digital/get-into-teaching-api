using GetIntoTeaching.Core.CrossCuttingConcerns.DependencyInjection;
using System.Collections.Concurrent;

namespace GetIntoTeaching.Core.CrossCuttingConcerns.Mediator
{
    /// <summary>
    /// 
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly ServiceFactory _serviceFactory;
        private readonly ConcurrentDictionary<Type, Type> _handlersMetadata;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceFactory"></param>
        /// <param name="handlersMetadata"></param>
        public Mediator(
            ServiceFactory serviceFactory,
            ConcurrentDictionary<Type, Type> handlersMetadata)
        {
            _serviceFactory = serviceFactory;
            _handlersMetadata = handlersMetadata;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<TResponse> Handle<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest<TResponse>
                => Handle<TRequest, TResponse>(request, CancellationToken.None);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest<TResponse>
        {
            ArgumentNullException.ThrowIfNull(request);

            Type? requestType = request.GetType();

            if (!_handlersMetadata.TryGetValue(requestType, out Type? handlerType)){
                throw new InvalidOperationException($"No handler found for {requestType.FullName}");
            }

            IHandler<TRequest, TResponse> handler =
                _serviceFactory
                    .GetInstanceWithCast<IHandler<TRequest, TResponse>>(handlerType);

            return handler.Handle(request, cancellationToken);
        }
    }
}