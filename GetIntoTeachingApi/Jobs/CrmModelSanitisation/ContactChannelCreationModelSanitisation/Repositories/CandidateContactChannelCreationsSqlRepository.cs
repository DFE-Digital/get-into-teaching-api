using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GetIntoTeachingApi.Models.Apply;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;

/// <summary>
/// Repository for retrieving and persisting contact channel creation data 
/// associated with a candidate. Wraps serialization logic and database interactions.
/// </summary>
public class CandidateContactChannelCreationsSqlRepository : ICandidateContactChannelCreationsRepository
{
    private readonly GetIntoTeachingDbContext _dbContext;

    /// <summary>
    /// Constructs the repository with a validated database context.
    /// </summary>
    /// <param name="dbContext">Entity Framework database context.</param>
    /// <exception cref="ArgumentNullException">Thrown if the context is null.</exception>
    public CandidateContactChannelCreationsSqlRepository(GetIntoTeachingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Retrieves deserialized contact channel creation records for a given candidate.
    /// Converts stored JSON payload back into <see cref="ContactChannelCreation"/> models.
    /// </summary>
    /// <param name="candidateId">The unique identifier for the candidate.</param>
    /// <returns>
    /// A collection of <see cref="ContactChannelCreation"/> models.
    /// If the candidate ID is invalid or no records exist, returns an empty sequence.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if candidateId is empty.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if deserialization fails due to corrupted or incompatible JSON content.
    /// </exception>
    public IEnumerable<ContactChannelCreation> GetContactChannelCreationsByCandidateId(Guid candidateId)
    {
        CandidateContactChannelCreations record = GetRawCandidateCreationChannels(candidateId);
        
        if (record == null || string.IsNullOrWhiteSpace(record.SerialisedContactCreationChannels))
        {
            return Enumerable.Empty<ContactChannelCreation>();
        }

        try
        {
            return record.SerialisedContactCreationChannels
                .DeserializeChangeTracked<IEnumerable<ContactChannelCreation>>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to deserialize ContactChannelCreations for CandidateId {candidateId}.", ex);
        }
    }

    /// <summary>
    /// Persists serialized contact channel creations for a candidate.
    /// Serializes the entity and updates the record in the database.
    /// </summary>
    /// <param name="saveRequest">Encapsulates candidate ID and channel creation data.</param>
    /// <returns>
    /// A <see cref="SaveResult"/> indicating success state and diagnostic messaging.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if candidateId is empty.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if serialization or database save fails.
    /// </exception>
    public SaveResult SaveContactChannelCreations(ContactChannelCreationSaveRequest saveRequest)
    {
        CandidateContactChannelCreations entity = new(
            candidateId: saveRequest.CandidateId,
            serialisedContactCreationChannels: saveRequest.GetContactChannelCreationJsonAsString());

        try
        {
            CandidateContactChannelCreations record =
                GetRawCandidateCreationChannels(saveRequest.CandidateId);

            if (record == null)
            {
                _dbContext.CandidateContactChannelCreations.Add(entity);
            }
            else
            {
                _dbContext.CandidateContactChannelCreations.Update(entity);
            }

            _dbContext.SaveChanges();

            return SaveResult.Create(
                isSuccessful: true,
                message: $"Successfully saved ContactChannelCreations for CandidateId {saveRequest.CandidateId}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to save ContactChannelCreations for CandidateId {saveRequest.CandidateId}.", ex);
        }
    }

    private CandidateContactChannelCreations GetRawCandidateCreationChannels(Guid candidateId)
    {
        if (candidateId == Guid.Empty)
        {
            throw new ArgumentException("CandidateId must not be empty.", nameof(candidateId));
        }

        return _dbContext.CandidateContactChannelCreations
            .AsNoTracking()
            .SingleOrDefault(c => c.CandidateId == candidateId);
    }
}
