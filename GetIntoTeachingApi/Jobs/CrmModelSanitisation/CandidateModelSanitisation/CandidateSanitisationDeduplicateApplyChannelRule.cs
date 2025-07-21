using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using System;
using System.Linq;

namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;

/// <summary>
/// Sanitisation rule that removes duplicate "CreatedOnApply" contact channel creation records
/// when the candidate already exists with such a record in CRM.
/// </summary>
public class CandidateSanitisationDeduplicateApplyChannelRule : ICrmModelSanitisationRule<Candidate>
{
    private readonly ICrmService _crmService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CandidateSanitisationDeduplicateApplyChannelRule"/> class.
    /// </summary>
    /// <param name="crmService">CRM service used to retrieve existing candidate records.</param>
    public CandidateSanitisationDeduplicateApplyChannelRule(ICrmService crmService)
    {
        _crmService = crmService ?? throw new ArgumentNullException(nameof(crmService));
    }

    /// <summary>
    /// Sanitises the candidate by removing duplicate "CreatedOnApply" contact channel creation records.
    /// </summary>
    /// <param name="updateCandidate">The candidate to sanitise.</param>
    /// <returns>
    /// The sanitised candidate with redundant "CreatedOnApply" records removed if necessary.
    /// </returns>
    public Candidate SanitiseCrmModel(Candidate model)
    {
        if (ShouldSkipApplyChannelSanitisation(model))
            return model;

        Candidate existingCandidate = _crmService.GetCandidate(model.Id.Value);

        if (ShouldSkipExistingCandidateSanitisation(existingCandidate))
            return model;

        RemoveApplyChannelCreations(model);

        return model;
    }

    /// <summary>
    /// Determines whether the candidate should skip apply channel sanitisation.
    /// </summary>
    /// <param name="candidate">The candidate to evaluate.</param>
    /// <returns>
    /// <c>true</c> if candidate is null, lacks an ID, or has no CreatedOnApply channel; otherwise, <c>false</c>.
    /// </returns>
    private static bool ShouldSkipApplyChannelSanitisation(Candidate candidate) =>
        candidate?.Id.HasValue != true || !HasCreatedOnApplyChannel(candidate);

    /// <summary>
    /// Determines whether apply channel sanitisation should be skipped for the existing candidate.
    /// </summary>
    /// <param name="candidate">The existing candidate to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the candidate is null or does not have a CreatedOnApply contact channel; otherwise, <c>false</c>.
    /// </returns>
    private static bool ShouldSkipExistingCandidateSanitisation(Candidate candidate) =>
        candidate == null || !HasCreatedOnApplyChannel(candidate);


    /// <summary>
    /// Determines whether the candidate contains any "CreatedOnApply" contact channel creation records.
    /// </summary>
    /// <param name="candidate">The candidate to inspect.</param>
    /// <returns>
    /// <c>true</c> if the candidate has at least one "CreatedOnApply" record; otherwise, <c>false</c>.
    /// </returns>
    private static bool HasCreatedOnApplyChannel(Candidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        return candidate.ContactChannelCreations.Any(cc =>
            cc.CreationChannelSourceId == ContactChannelConstants.ApplySourceId &&
            cc.CreationChannelServiceId == ContactChannelConstants.CreatedOnApplyServiceId);
    }

    /// <summary>
    /// Removes all "CreatedOnApply" contact channel creation records from the candidate.
    /// </summary>
    /// <param name="candidate">The candidate to modify.</param>
    private static void RemoveApplyChannelCreations(Candidate candidate)
    {
        candidate.ContactChannelCreations.RemoveAll(cc =>
            cc.CreationChannelSourceId == ContactChannelConstants.ApplySourceId &&
            cc.CreationChannelServiceId == ContactChannelConstants.CreatedOnApplyServiceId);
    }

    /// <summary>
    /// Contains constants for identifying specific contact channel creation sources and services.
    /// </summary>
    internal static class ContactChannelConstants
    {
        /// <summary>
        /// Represents the source ID for the 'Apply' contact channel.
        /// </summary>
        public const int ApplySourceId = (int)ContactChannelCreation.CreationChannelSource.Apply;

        /// <summary>
        /// Represents the service ID for the 'CreatedOnApply' channel creation method.
        /// </summary>
        public const int CreatedOnApplyServiceId = (int)ContactChannelCreation.CreationChannelService.CreatedOnApply;
    }
}
