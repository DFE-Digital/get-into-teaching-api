using System;
using System.Linq;

namespace GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;

/// <summary>
/// Applies a sanitisation rule to remove duplicate contact channel creation records
/// for a candidate model based on matching source/service/activity identifiers.
/// </summary>
public class ContactChannelCreationDuplicateSanitisationRule
    : ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>
{
    /// <summary>
    /// Sanitises the incoming CRM model by removing redundant creation channel records
    /// if a matching persisted instance exists for the candidate.
    /// </summary>
    /// <param name="model">
    /// The request wrapper encapsulating a candidate's contact channel creation model and prior records.
    /// </param>
    /// <returns>
    /// The sanitized wrapper with the CreationChannel either removed or preserved,
    /// based on duplication logic.
    /// </returns>
    public ContactChannelCreationSanitisationRequestWrapper SanitiseCrmModel(
        ContactChannelCreationSanitisationRequestWrapper model)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(model.CreationChannel);

        // Checks for duplication based on domain identifiers and removes if matched
        if (HasMatchingCreationChannel(model))
        {
            model.RemoveCreationChannel();
            return model;
        }

        model.PreserveCreationChannel();
        return model;
    }

    /// <summary>
    /// Determines whether the current CreationChannel has a matching record
    /// already associated with the candidate's contact history.
    /// </summary>
    /// <param name="model">The wrapper containing candidate history and incoming data.</param>
    /// <returns>True if a match is found; false otherwise.</returns>
    private static bool HasMatchingCreationChannel(ContactChannelCreationSanitisationRequestWrapper model)
    {
        ArgumentNullException.ThrowIfNull(model);

        // Compares composite key (source/service/activity) to detect duplicates.
        return model.CandidateContactChannelCreations.Any(creationChannel =>
            creationChannel.CreationChannelSourceId == model.CreationChannel.CreationChannelSourceId &&
            creationChannel.CreationChannelServiceId == model.CreationChannel.CreationChannelServiceId &&
            creationChannel.CreationChannelActivityId == model.CreationChannel.CreationChannelActivityId);
    }
}
