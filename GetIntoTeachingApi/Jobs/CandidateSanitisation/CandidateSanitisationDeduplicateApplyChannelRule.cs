using System;
using System.Linq;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;


public class CandidateSanitisationDeduplicateApplyChannelRule : ICandidateSanitisationRule
{
    private readonly ICrmService _crm;
    
    public CandidateSanitisationDeduplicateApplyChannelRule(ICrmService crm)
    {
        _crm = crm;
        
    }
    
    public Candidate SanitiseCandidate(Candidate updateCandidate)
    {
        // This rule will check to see if the updateCandidate object to be upserted has a local CreatedOnApply Candidate Creation Channel record.
        if (HasCreatedOnApplyContactChannelCreationRecord(updateCandidate) && updateCandidate.Id.HasValue)
        {
            Candidate crmCandidate = _crm.GetCandidate(updateCandidate.Id.Value);

            if (crmCandidate != null && HasCreatedOnApplyContactChannelCreationRecord(crmCandidate))
            {
                // we need to remove the CreatedOnApply ContactCreationChannel record on the update candidate to
                // prevent duplicates
                updateCandidate.ContactChannelCreations.RemoveAll(contactChannelCreation =>
                    contactChannelCreation.CreationChannelSourceId ==
                    (int)ContactChannelCreation.CreationChannelSource.Apply &&
                    contactChannelCreation.CreationChannelServiceId ==
                    (int)ContactChannelCreation.CreationChannelService.CreatedOnApply);
            }
        }

        return updateCandidate;
    }
    
    
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
    private static bool HasCreatedOnApplyContactChannelCreationRecord(Candidate updateCandidate)
    {
        ArgumentNullException.ThrowIfNull(updateCandidate);

        return updateCandidate.ContactChannelCreations.Any(contactChannelCreation =>
            contactChannelCreation.CreationChannelSourceId == (int)ContactChannelCreation.CreationChannelSource.Apply &&
            contactChannelCreation.CreationChannelServiceId == (int)ContactChannelCreation.CreationChannelService.CreatedOnApply);
    }
}