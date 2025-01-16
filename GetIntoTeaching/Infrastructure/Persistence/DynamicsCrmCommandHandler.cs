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

        public DynamicsCrmCommandHandler(ICrmServiceClientProvider crmClientProvider) :
            base(crmClientProvider)
        {
            ServiceClient? serviceClient =
                base.CrmClientProvider
                    .GetCrmServiceClient("DynamicsCrmServiceClient") as ServiceClient;

            _crmServiceClient =
                serviceClient ??
                throw new ArgumentNullException(nameof(crmClientProvider));
        }

        public override TResult ExecuteCommand<TCommandQuery, TResult>(TCommandQuery query)
        {
            using (var crmServiceContext = new OrganizationServiceContext(_crmServiceClient))
            {
                crmServiceContext.SaveChanges(); // This performs the actual call against the CRM 
            }

            return (TResult)true;
        }
    }
}
