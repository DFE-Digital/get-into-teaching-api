using GetIntoTeachingApi.Models.Crm;
using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;

/// <summary>
/// Repository abstraction for retrieving candidate-specific contact channel creation records.
/// Aligns with the persistence boundary in Clean Architecture, enabling decoupled querying.
/// </summary>
public interface ICandidateContactChannelCreationsRepository
{
    /// <summary>
    /// Fetches the collection of contact channel creation entities associated with the given candidate ID.
    /// </summary>
    /// <param name="candidateId">
    /// The unique identifier representing the candidate whose channel creation records are being queried.
    /// </param>
    /// <returns>
    /// A sequence of <see cref="ContactChannelCreation"/> entities related to the candidate.
    /// </returns>
    IEnumerable<ContactChannelCreation> GetContactChannelCreationsByCandidateId(Guid candidateId);

    /// <summary>
    /// Executes a save operation for one or more contact channel creation requests.
    /// Returns a structured result indicating overall success and item-level feedback.
    /// </summary>
    /// <param name="saveRequest">
    /// The request payload containing metadata required to persist channel creation records,
    /// such as candidate ID, channel types (e.g., SMS, Email), and timestamps.
    /// </param>
    /// <returns>
    /// A <see cref="SaveResult"/> instance capturing success flags, contextual messages,
    /// and granular errors where applicable.
    /// </returns>
    SaveResult SaveContactChannelCreations(ContactChannelCreationSaveRequest saveRequest);
}