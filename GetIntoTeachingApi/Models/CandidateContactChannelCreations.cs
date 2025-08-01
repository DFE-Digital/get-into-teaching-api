using System;

namespace GetIntoTeachingApi.Models;

/// <summary>
/// Represents a persisted record of a candidate's contact channel creation history,
/// stored as a serialized JSON string for audit or transmission.
/// </summary>
public class CandidateContactChannelCreations
{
    /// <summary>
    /// Gets or sets the unique identifier for the candidate associated with this entity.
    /// Typically acts as a foreign key or routing anchor across CRM models.
    /// </summary>
    public Guid CandidateId { get; set; }


    /// <summary>
    /// Serialized JSON string representing the contact channel creation payload.
    /// Expected to originate from <see cref="ContactChannelCreationSaveRequest"/> serialization.
    /// </summary>
    public string SerialisedContactCreationChannels { get; set; }

    /// <summary>
    /// Constructs the candidate channel creation record with identifier and serialized payload.
    /// </summary>
    /// <param name="candidateId">Candidate identifier to associate with the record.</param>
    /// <param name="serialisedContactCreationChannels">Serialized JSON string of contact channel records.</param>
    public CandidateContactChannelCreations(Guid candidateId, string serialisedContactCreationChannels)
    {
        CandidateId = candidateId;
        SerialisedContactCreationChannels = serialisedContactCreationChannels;
    }
}