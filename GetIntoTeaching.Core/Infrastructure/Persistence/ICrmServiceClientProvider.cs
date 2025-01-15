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
        ICrmServiceClient GetCrmServiceClient(string clientServiceKey);
    }
}
