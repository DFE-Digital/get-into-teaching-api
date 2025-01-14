using GetIntoTeaching.Core.CrossCuttingConcerns.Mediator;
using GetIntoTeaching.Core.Infrastructure.BackgroundProcessing;

namespace GetIntoTeaching.Infrastructure.Persistence.CandidateBackgroundProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class HangfireBackgroundProcessingService : IBackgroundJobProcessService
    {
        private readonly IMediator _backgroundProcessorMediator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backgroundProcessorMediator"></param>
        public HangfireBackgroundProcessingService(IMediator backgroundProcessorMediator)
        {
            _backgroundProcessorMediator = backgroundProcessorMediator;
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
        { 
            // TODO: we need to handle the logging

            // TODO: we need to handle the retries and limits

            ArgumentNullException.ThrowIfNull(request);
            return Handle<TRequest, TResponse>(request, CancellationToken.None);
        }
            
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest<TResponse>
        {
           return _backgroundProcessorMediator.Handle<TRequest, TResponse>(request, cancellationToken);
        }
    }
}
