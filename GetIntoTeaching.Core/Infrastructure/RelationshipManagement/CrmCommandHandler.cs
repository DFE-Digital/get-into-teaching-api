namespace GetIntoTeaching.Core.Infrastructure.RelationshipManagement
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCommandQuery"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class CrmCommandHandler<TCommandQuery, TResult> :
        ICrmCommandHandler<TCommandQuery, TResult>
            where TCommandQuery : ICrmCommandQuery<TResult>
        {
            /// <summary>
            /// 
            /// </summary>
            protected ICrmServiceClientProvider CrmClientProvider { get; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="crmClientProvider"></param>
            protected CrmCommandHandler(ICrmServiceClientProvider crmClientProvider)
            {
                CrmClientProvider = crmClientProvider;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="query"></param>
            /// <returns></returns>
            public abstract Func<TResult> ExecuteCommand(TCommandQuery query);
        }
}
