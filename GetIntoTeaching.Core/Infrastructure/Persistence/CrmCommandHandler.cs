namespace GetIntoTeaching.Core.Infrastructure.Persistence
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CrmCommandHandler : ICrmCommandHandler
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
        public abstract TResult ExecuteCommand<TCommandQuery, TResult>(TCommandQuery query)
            where TCommandQuery : ICrmCommandQuery<TResult>;
    }
}
