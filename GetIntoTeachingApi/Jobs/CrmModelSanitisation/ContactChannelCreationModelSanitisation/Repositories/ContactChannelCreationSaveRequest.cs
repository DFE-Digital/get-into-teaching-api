using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Utils;
using System;
using System.Collections.Generic;
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
    public ContactChannelCreation ContactChannelCreation { get; private set; }


    /// <summary>
    /// Read-only collection of contact channel creation models to persist.
    /// Formed via safe merging and null-guarded initialization.
    /// </summary>
    private readonly ReadOnlyCollection<ContactChannelCreation> _candidateContactChannelCreations;

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
        if (candidateId == Guid.Empty)
        {
            throw new ArgumentException("CandidateId must not be empty.", nameof(candidateId));
        }

        CandidateId = candidateId;
        ContactChannelCreation = candidateContactChannelCreation ?? throw new ArgumentNullException(nameof(candidateContactChannelCreation));

        // Merge one-off creation with existing records
        _candidateContactChannelCreations =
            MergeContactChannelCreations(candidateContactChannelCreation, candidateContactChannelCreations);
    }

    /// <summary>
    /// Serializes the merged collection of channel creations to a JSON string.
    /// Enforces fail-fast behavior on serialization errors.
    /// </summary>
    /// <returns>JSON representation of the contact channel creations.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if serialization fails due to nulls or incompatible models.
    /// </exception>
    public string GetContactChannelCreationJsonAsString()
    {
        try
        {
            return _candidateContactChannelCreations.SerializeChangeTracked();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to serialize ContactChannelCreation for CandidateId {CandidateId}.", ex);
        }
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

    /// <summary>
    /// Safely merges one new creation record into an existing read-only collection.
    /// Avoids mutability or null access violations.
    /// </summary>
    /// <param name="candidateContactChannelCreation">New channel creation to include.</param>
    /// <param name="contactChannelCreations">Existing collection to merge with.</param>
    /// <returns>A combined and immutable collection of channel records.</returns>
    private static ReadOnlyCollection<ContactChannelCreation> MergeContactChannelCreations(
        ContactChannelCreation candidateContactChannelCreation,
        ReadOnlyCollection<ContactChannelCreation> contactChannelCreations)
    {
        List<ContactChannelCreation> mergedContactChannelCreations = [];
        mergedContactChannelCreations.AddRange(contactChannelCreations);
        mergedContactChannelCreations.Add(candidateContactChannelCreation);

        return mergedContactChannelCreations.AsReadOnly();
    }
}
