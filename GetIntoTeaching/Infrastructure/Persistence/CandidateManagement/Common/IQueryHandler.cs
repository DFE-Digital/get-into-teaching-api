using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace GetIntoTeaching.Infrastructure.Persistence.CandidateManagement.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQueryHandler
    {
        /// <summary>
        /// Performs a query by creating a LINQ query for entities over the precribed CRM, utilising the
        /// lambda expressions provisioned for the selector (projects each element of the specified
        /// sequence into a new form), and predicate (filters a sequence of values based on the
        /// criteria specified).
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of resource to return in the <see cref="ItemResponse{TItem}"/>.
        /// </typeparam>
        /// <param name="containerKey">
        /// Represents a container key value in the Azure Cosmos DB service.
        /// </param>
        /// <param name="selector">
        /// Lambda expression used to define what elements to project to the specified sequence.
        /// </param>
        /// <param name="predicate">
        /// Lambda expression used to define the predicates on which to filter a sequence of values
        /// based on the criteria specified.
        /// </param>
        /// <returns>
        /// A configured instance of the specified generic type which encapsulates the
        /// requested CRM entity item.
        /// </returns>
        ImmutableList<IExtensibleDataObject> ReadItems(
            string entityName,
            Expression<Func<IExtensibleDataObject, IExtensibleDataObject>> selector,
            Expression<Func<IExtensibleDataObject, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public ImmutableList<IExtensibleDataObject> ReadItems(IExtensibleDataObject query);
    }
}