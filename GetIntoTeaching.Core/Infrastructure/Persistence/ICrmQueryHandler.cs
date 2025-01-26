namespace GetIntoTeaching.Core.Infrastructure.Persistence
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICrmQueryHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TResult> ExecuteQuery<TQuery, TResult>(TQuery query)
            where TQuery : ICrmQuery<TResult>
            where TResult : new();
    }
}