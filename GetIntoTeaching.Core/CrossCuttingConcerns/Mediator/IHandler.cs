namespace GetIntoTeaching.Core.CrossCuttingConcerns.Mediator
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IHandler<in TRequest, TResponse>
        where TRequest :
        IRequest<TResponse>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken = default);
    }
}