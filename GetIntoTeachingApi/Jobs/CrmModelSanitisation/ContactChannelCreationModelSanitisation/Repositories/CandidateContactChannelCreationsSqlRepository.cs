using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;

/// <summary>
/// SQL-backed repository for retrieving and persisting serialized contact channel creations
/// linked to individual candidates. Validates inputs and guards against serialization faults.
/// </summary>
public class CandidateContactChannelCreationsSqlRepository : ICandidateContactChannelCreationsRepository
{
    private readonly GetIntoTeachingDbContext _dbContext;

    /// <summary>
    /// Constructs the repository with a validated EF Core database context.
    /// </summary>
    /// <param name="dbContext">An initialized EF database context.</param>
    /// <exception cref="ArgumentNullException">Thrown if the context is null.</exception>
    public CandidateContactChannelCreationsSqlRepository(GetIntoTeachingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Retrieves and deserializes the contact channel creations for a candidate.
    /// </summary>
    /// <param name="candidateId">The candidate's unique identifier.</param>
    /// <returns>
    /// A collection of <see cref="ContactChannelCreation"/> records.
    /// Returns an empty list if the record is missing or the stored payload is null or empty.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if candidateId is empty.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if deserialization fails due to corrupted or incompatible content.
    /// </exception>
    public IEnumerable<ContactChannelCreation> GetContactChannelCreationsByCandidateId(Guid candidateId)
    {
        var record = GetRawCandidateCreationChannels(candidateId);

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
    /// Persists or updates serialized contact channel creation data for a candidate.
    /// Uses upsert semantics based on existence check.
    /// </summary>
    /// <param name="saveRequest">Encapsulates candidate ID and serialized channel records.</param>
    /// <returns>
    /// A <see cref="SaveResult"/> indicating the success of the save operation.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the candidate ID is empty, guarding against invalid model state.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when persistence fails due to database or serialization errors.
    /// </exception>
    public SaveResult SaveContactChannelCreations(ContactChannelCreationSaveRequest saveRequest)
    {
        // Ensure input contains a valid candidate identifier
        if (saveRequest.CandidateId == Guid.Empty)
        {
            throw new ArgumentException("CandidateId must not be empty.");
        }

        // Construct the entity to persist using serialized payload
        var entity = new CandidateContactChannelCreations(
            candidateId: saveRequest.CandidateId,
            serialisedContactCreationChannels: saveRequest.GetContactChannelCreationJsonAsString());

        try
        {
            // Retrieve existing record if it exists — determines upsert strategy
            var record = GetRawCandidateCreationChannels(saveRequest.CandidateId);

            // Retrieve tracked DbSet to perform stateful persistence
            var dbSet = _dbContext.CandidateContactChannelCreations;

            // Conditionally upsert — adds new or updates existing
            // Calling State.ToString() activates tracking without altering execution
            (record == null ? dbSet.Add(entity) : dbSet.Update(entity)).State.ToString();

            // Persist changes to the database
            _dbContext.SaveChanges();

            // Return a successful save result with candidate context
            return SaveResult.Create(
                isSuccessful: true,
                message: $"Successfully saved ContactChannelCreations for CandidateId {saveRequest.CandidateId}");
        }
        catch (Exception ex)
        {
            // Fail-fast with high-fidelity diagnostics if persistence fails
            throw new InvalidOperationException(
                $"Failed to save ContactChannelCreations for CandidateId {saveRequest.CandidateId}.", ex);
        }
    }

    /// <summary>
    /// Retrieves the raw entity storing contact channel JSON payload for the specified candidate.
    /// </summary>
    /// <param name="candidateId">The identifier used to locate the record.</param>
    /// <returns>
    /// A <see cref="CandidateContactChannelCreations"/> entity if found; otherwise null.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if candidateId is empty.</exception>
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
