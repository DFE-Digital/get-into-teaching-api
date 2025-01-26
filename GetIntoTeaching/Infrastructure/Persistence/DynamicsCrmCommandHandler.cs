using GetIntoTeaching.Core.Infrastructure.Persistence;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeaching.Infrastructure.Persistence
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DynamicsCrmCommandHandler : ICrmCommandHandler
    {
        private readonly ServiceClient _crmServiceClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="crmClientProvider"></param>
        /// <param name="dynamicsCrmServiceClientKey"></param>
        public DynamicsCrmCommandHandler(
            ICrmServiceClientProvider crmClientProvider, string dynamicsCrmServiceClientKey)
        {
            ServiceClient serviceClient =
                crmClientProvider.GetCrmServiceClient<ServiceClient>(dynamicsCrmServiceClientKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommandQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public TResult ExecuteCommand<TCommandQuery, TResult>(TCommandQuery query)
            where TCommandQuery : ICrmCommandQuery<TResult>
            where TResult : new()
        {
            using (var crmServiceContext = new OrganizationServiceContext(_crmServiceClient))
            {
                // invoke query here.....

                crmServiceContext.SaveChanges(); 
            }

            TResult result = new();

            return result;
        }
    }
}
