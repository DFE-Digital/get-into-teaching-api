using GetIntoTeaching.Core.Infrastructure.Persistence;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeaching.Infrastructure.Persistence
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DynamicsCrmCommandHandler : CrmCommandHandler
    {
        private readonly ServiceClient _crmServiceClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="crmClientProvider"></param>
        /// <param name="crmServiceClientKey"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DynamicsCrmCommandHandler(
            ICrmServiceClientProvider crmClientProvider,
            string dynamicsCrmServiceClientKey) : base(crmClientProvider)
        {
            ServiceClient? serviceClient =
                base.CrmClientProvider
                    .GetCrmServiceClient(dynamicsCrmServiceClientKey) as ServiceClient;

            _crmServiceClient =
                serviceClient ?? throw new ArgumentNullException(nameof(crmClientProvider));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommandQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public override TResult ExecuteCommand<TCommandQuery, TResult>(TCommandQuery query)
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
