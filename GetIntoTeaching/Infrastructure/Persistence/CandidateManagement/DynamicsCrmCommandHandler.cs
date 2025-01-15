using GetIntoTeaching.Core.Infrastructure.RelationshipManagement;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeaching.Infrastructure.Persistence.CandidateManagement
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DynamicsCrmCommandHandler : CrmCommandHandler<object, object>
    {
        private readonly ServiceClient _crmServiceClient;

        public DynamicsCrmCommandHandler(IOrganizationService crmServiceClient)
        {
            _crmServiceClient = (ServiceClient)crmServiceClient;
        }

        public override object ExecuteCommand(object query)
        {
            using (var crmServiceContext = new OrganizationServiceContext(_crmServiceClient))
            {
                crmServiceContext.SaveChanges(); // This performs the actual call against the CRM 
            }
        }
    }
}
