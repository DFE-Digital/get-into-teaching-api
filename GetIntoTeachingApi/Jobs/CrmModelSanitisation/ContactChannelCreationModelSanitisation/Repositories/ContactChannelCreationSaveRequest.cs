using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Utils;
using System;
using System.Collections.ObjectModel;

namespace GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;

/// <summary>
/// Represents a save payload for contact channel creation records associated with a candidate.
/// Ensures identifier validity and serialization reliability.
/// </summary>
public class ContactChannelCreationSaveRequest
{
    /// <summary>
    /// The unique identifier representing the candidate for whom channels are being saved.
    /// </summary>
    public Guid CandidateId { get; }

    /// <summary>
    /// Internal collection of contact channel creations to persist.
    /// Initialized with a null-safe fallback to avoid access violations.
    /// </summary>
    private readonly ReadOnlyCollection<ContactChannelCreation> _candidateContactChannelCreations;

    /// <summary>
    /// Constructs a save request payload with strict validation against null and empty inputs.
    /// </summary>
    /// <param name="candidateId">The candidate's unique identifier.</param>
    /// <param name="candidateContactChannelCreations">List of contact channel records to persist.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="candidateId"/> is empty.</exception>
    public ContactChannelCreationSaveRequest(
        Guid candidateId,
        ReadOnlyCollection<ContactChannelCreation> candidateContactChannelCreations)
    {
        if (candidateId == Guid.Empty)
        {
            throw new ArgumentException("CandidateId must not be empty.", nameof(candidateId));
        }

        CandidateId = candidateId;

        _candidateContactChannelCreations = candidateContactChannelCreations ??
            new ReadOnlyCollection<ContactChannelCreation>(Array.Empty<ContactChannelCreation>());
    }

    /// <summary>
    /// Serializes the contact channel creation records to a JSON string.
    /// Throws if serialization fails, enforcing data integrity.
    /// </summary>
    /// <returns>JSON representation of the contact channel creations.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if serialization fails due to unexpected content or null references.
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
    /// Factory method to create a validated <see cref="ContactChannelCreationSaveRequest"/>.
    /// Centralizes construction logic for consistency.
    /// </summary>
    /// <param name="candidateId">The candidate's identifier.</param>
    /// <param name="candidateContactChannelCreations">Channel records to save.</param>
    /// <returns>A validated and constructed instance of the save request.</returns>
    public static ContactChannelCreationSaveRequest Create(
        Guid candidateId,
        ReadOnlyCollection<ContactChannelCreation> candidateContactChannelCreations)
    {
        return new ContactChannelCreationSaveRequest(candidateId, candidateContactChannelCreations);
    }
}
