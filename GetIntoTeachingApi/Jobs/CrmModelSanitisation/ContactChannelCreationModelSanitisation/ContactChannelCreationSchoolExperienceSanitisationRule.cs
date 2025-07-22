using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using System;
using System.Linq;

namespace GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;

public class ContactChannelCreationSchoolExperienceSanitisationRule : ICrmModelSanitisationRule<ContactChannelCreation>
{
    public ContactChannelCreation SanitiseCrmModel(ContactChannelCreation model)
    {
        
    }
    
    /// <summary>
    /// Determines whether the candidate should skip school experience channel sanitisation.
    /// </summary>
    /// <param name="candidate">The candidate to evaluate.</param>
    /// <returns>
    /// <c>true</c> if candidate is null, lacks an ID, or has no SchoolExperience channel; otherwise, <c>false</c>.
    /// </returns>
    private static bool ShouldSkipSchoolExperienceChannelSanitisation(Candidate candidate) =>
        !candidate?.Id.HasValue || !HasCreatedOnSchoolExperienceChannel(candidate);
    
    /// <summary>
    /// Determines whether the candidate contains any "SchoolExperience" contact channel creation records.
    /// </summary>
    /// <param name="candidate">The candidate to inspect.</param>
    /// <returns>
    /// <c>true</c> if the candidate has at least one "SchoolExperience" record; otherwise, <c>false</c>.
    /// </returns>
    private static bool HasCreatedOnSchoolExperienceChannel(Candidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        return candidate.ContactChannelCreations.Any(cc =>
            cc.CreationChannelSourceId == ContactChannelConstants.SchoolExperienceSourceId &&
            cc.CreationChannelServiceId == ContactChannelConstants.CreatedOnSchoolExperienceServiceId);
    }
    
    /// <summary>
    /// Contains constants for identifying specific contact channel creation sources and services.
    /// </summary>
    internal static class ContactChannelConstants
    {
        /// <summary>
        /// Represents the source ID for the 'SchoolExperience' contact channel.
        /// </summary>
        public const int SchoolExperienceSourceId = (int)ContactChannelCreation.CreationChannelSource.SchoolExperience;

        /// <summary>
        /// Represents the service ID for the 'CreatedOnSchoolExperience' channel creation method.
        /// </summary>
        public const int CreatedOnSchoolExperienceServiceId = (int)ContactChannelCreation.CreationChannelService.CreatedOnSchoolExperience;
    }
}