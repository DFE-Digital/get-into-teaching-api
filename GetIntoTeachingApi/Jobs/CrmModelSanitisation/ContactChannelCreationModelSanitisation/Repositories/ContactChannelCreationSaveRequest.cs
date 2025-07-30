using GetIntoTeachingApi.Models.Crm;
using System;
using System.Collections.ObjectModel;

namespace GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;

/// <summary>
/// Encapsulates a candidate's contact channel creation payload for persistence.
/// Combines identifier validation, serialization utility, and record consolidation.
/// </summary>
public class ContactChannelCreationSaveRequest
{
    /// <summary>
    /// Uniquely identifies the candidate associated with this save request.
    /// Immutable once constructed.
    /// </summary>
    public Guid CandidateId { get; }

    /// <summary>
    /// Exposes a single instance of the ContactChannelCreation domain model,
    /// allowing external consumers to read it but restricting mutation to within this class.
    /// This aligns with encapsulation principles and supports controlled lifecycle management.
    /// </summary>
    public ContactChannelCreation ContactChannelCreation { get; }

    /// <summary>
    /// Constructs a save request with candidate ID and merged channel creations.
    /// Ensures ID validity and guards against null collections.
    /// </summary>
    /// <param name="candidateId">The candidate's unique identifier.</param>
    /// <param name="candidateContactChannelCreation">A newly created channel record to include.</param>
    /// <param name="candidateContactChannelCreations">Existing records to merge into payload.</param>
    /// <exception cref="ArgumentException">Thrown if candidateId is Guid.Empty.</exception>
    public ContactChannelCreationSaveRequest(
        Guid candidateId,
        ContactChannelCreation candidateContactChannelCreation,
        ReadOnlyCollection<ContactChannelCreation> candidateContactChannelCreations)
    {
        CandidateId = candidateId;
        ContactChannelCreation = candidateContactChannelCreation ??
            throw new ArgumentNullException(nameof(candidateContactChannelCreation));
    }

    /// <summary>
    /// Factory method for consistent instantiation.
    /// Enforces validation and encapsulates construction logic.
    /// </summary>
    public static ContactChannelCreationSaveRequest Create(
        Guid candidateId,
        ContactChannelCreation candidateContactChannelCreation,
        ReadOnlyCollection<ContactChannelCreation> candidateContactChannelCreations) =>
            new(candidateId, candidateContactChannelCreation, candidateContactChannelCreations);
}
