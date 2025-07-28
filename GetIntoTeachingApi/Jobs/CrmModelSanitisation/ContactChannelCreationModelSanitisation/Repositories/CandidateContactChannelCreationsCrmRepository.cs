using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;

/// <summary>
/// CRM-backed repository for managing candidate <see cref="ContactChannelCreation"/> entities.
/// Supports querying and persisting creation channels associated with a candidate.
/// </summary>
public class CandidateContactChannelCreationsCrmRepository : ICandidateContactChannelCreationsRepository
{
    /// <summary>
    /// CRM service interface for persisting and retrieving CRM entities.
    /// </summary>
    private readonly ICrmService _crmService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CandidateContactChannelCreationsCrmRepository"/> class.
    /// </summary>
    /// <param name="crmService">Service adapter to interact with the CRM.</param>
    public CandidateContactChannelCreationsCrmRepository(ICrmService crmService)
    {
        _crmService = crmService;
    }

    /// <summary>
    /// Retrieves a collection of <see cref="ContactChannelCreation"/> entities for a given candidate.
    /// </summary>
    /// <param name="candidateId">Unique identifier for the candidate.</param>
    /// <returns>List of candidate's contact channel creations, or an empty list if none exist.</returns>
    public IEnumerable<ContactChannelCreation> GetContactChannelCreationsByCandidateId(Guid candidateId)
    {
        IEnumerable<ContactChannelCreation> contactChannelCreations = GetRawCandidateCreationChannels(candidateId);
        return (contactChannelCreations?.Any() != true) ? [] : contactChannelCreations;
    }

    /// <summary>
    /// Saves a <see cref="ContactChannelCreation"/> entity tied to a specific candidate.
    /// </summary>
    /// <param name="saveRequest">The save request containing candidate ID and the channel creation payload.</param>
    /// <returns>A result indicating success or failure of the save operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the candidate ID is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when persistence fails due to an exception.</exception>
    public SaveResult SaveContactChannelCreations(ContactChannelCreationSaveRequest saveRequest)
    {
        if (saveRequest.CandidateId == Guid.Empty)
        {
            throw new ArgumentException("CandidateId must not be empty.");
        }

        try
        {
            _crmService.Save(saveRequest.ContactChannelCreation);
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

    /// <summary>
    /// Gets raw <see cref="ContactChannelCreation"/> data for a candidate.
    /// Internally reuses the main retrieval method for consistency.
    /// </summary>
    /// <param name="candidateId">Unique identifier for the candidate.</param>
    /// <returns>List of contact channel creations.</returns>
    public IEnumerable<ContactChannelCreation> GetRawCandidateCreationChannels(Guid candidateId) => GetContactChannelCreationsByCandidateId(candidateId);
}
