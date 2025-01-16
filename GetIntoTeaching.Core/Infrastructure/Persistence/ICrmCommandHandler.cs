namespace GetIntoTeaching.Core.Infrastructure.Persistence
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICrmCommandHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommandQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        TResult ExecuteCommand<TCommandQuery, TResult>(TCommandQuery query)
            where TCommandQuery : ICrmCommandQuery<TResult>;
    }
}
