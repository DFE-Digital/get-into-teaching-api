using GetIntoTeaching.Infrastructure.Persistence.CandidateManagement.Common;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace GetIntoTeaching.Infrastructure.Persistence.CandidateManagement
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DynamicsCrmQueryHandler : ICrmQueryHandler
    {
        private readonly ServiceClient _dataverseServiceClient;
        private readonly OrganizationServiceContext _serviceContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataverseService">
        /// 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// 
        /// </exception>
        public DynamicsCrmQueryHandler(IOrganizationService dataverseService)
        {
            _dataverseServiceClient = dataverseService as ServiceClient ??
                throw new ArgumentNullException(nameof(dataverseService));

            _serviceContext = new OrganizationServiceContext(dataverseService);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ImmutableList<IExtensibleDataObject> ReadItems(
            string entityName,
            Expression<Func<IExtensibleDataObject, IExtensibleDataObject>> selector,
            Expression<Func<IExtensibleDataObject, bool>> predicate)
        {
            ArgumentException.ThrowIfNullOrEmpty(entityName);

            return _serviceContext.CreateQuery(entityName)
                .Where(predicate)
                .Select(selector).ToImmutableList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ImmutableList<IExtensibleDataObject> ReadItems(IExtensibleDataObject query)
        {
            ArgumentNullException.ThrowIfNull(query);

            QueryBase? queryBase = query as QueryBase ??
                throw new ArgumentNullException(nameof(query));

            EntityCollection entityCollection =
                _dataverseServiceClient.RetrieveMultiple(queryBase);

            return entityCollection.Entities.ToImmutableList<IExtensibleDataObject>();
        }
    }
}
