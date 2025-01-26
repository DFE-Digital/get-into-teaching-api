namespace GetIntoTeaching.Core.Infrastructure.Persistence
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICrmServiceClientProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientServiceKey"></param>
        /// <returns></returns>
        TServiceClient GetCrmServiceClient<TServiceClient>(string clientServiceKey) where TServiceClient : class;
    }
}
