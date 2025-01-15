namespace GetIntoTeaching.Core.Infrastructure.RelationshipManagement
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface ICrmQueryHandler<TQuery, TResult>
        where TQuery : ICrmQuery<TResult>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="query"></param>
            /// <returns></returns>
            Task<TResult> ExecuteQuery(TQuery query);
        }
}