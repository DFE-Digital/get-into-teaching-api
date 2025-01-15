namespace GetIntoTeaching.Core.Infrastructure.RelationshipManagement
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCommandQuery"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface ICrmCommandHandler<TCommandQuery, TResult>
        where TCommandQuery : ICrmCommandQuery<TResult>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="query"></param>
            /// <returns></returns>
            Func<TResult> ExecuteCommand(TCommandQuery query);
        }
}
