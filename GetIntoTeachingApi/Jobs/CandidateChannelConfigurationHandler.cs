using System;
using GetIntoTeachingApi.Models.Crm;
using System.Linq;

namespace GetIntoTeachingApi.Jobs;

/// <summary>
/// Handles candidate contact channel configuration logic.
/// </summary>
public class CandidateChannelConfigurationHandler : ICandidateChannelConfigurationHandler
{
    /// <summary>
    /// Determines whether the specified candidate lacks a contact channel creation record
    /// from Apply via CreatedOnApply service.
    /// </summary>
    /// <param name="candidate">The candidate to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the candidate has no matching contact channel creation record;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="candidate"/> is null.</exception>
    public bool DoesNotHaveAContactChannelCreationRecord(Candidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        return !candidate.ContactChannelCreations.Any(contactChannelCreation =>
            contactChannelCreation.CreationChannelSourceId == (int)ContactChannelCreation.CreationChannelSource.Apply &&
            contactChannelCreation.CreationChannelServiceId == (int)ContactChannelCreation.CreationChannelService.CreatedOnApply);
    }

    /// <summary>
    /// Invokes contact channel configuration for the candidate using the specified wrapper.
    /// </summary>
    /// <param name="wrappedCandidate">The wrapper containing candidate and channel context.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedCandidate"/> is null.</exception>
    public void InvokeConfigureChannel(ContactChannelCandidateWrapper wrappedCandidate)
    {
        ArgumentNullException.ThrowIfNull(wrappedCandidate);
        
        wrappedCandidate.ScopedCandidate.ConfigureChannel(
            candidateId: wrappedCandidate.ScopedCandidate.Id,
            primaryContactChannel: wrappedCandidate
        );
    }
}

/// <summary>
/// Interface for configuring candidate contact channel data.
/// </summary>
public interface ICandidateChannelConfigurationHandler
{
    /// <summary>
    /// Checks if the candidate lacks an Apply channel creation record.
    /// </summary>
    /// <param name="candidate">The candidate to check.</param>
    /// <returns><c>true</c> if the record is missing; otherwise, <c>false</c>.</returns>
    bool DoesNotHaveAContactChannelCreationRecord(Candidate candidate);

    /// <summary>
    /// Configures the contact channel for the specified candidate wrapper.
    /// </summary>
    /// <param name="wrappedCandidate">The wrapped candidate object.</param>
    void InvokeConfigureChannel(ContactChannelCandidateWrapper wrappedCandidate);
}
