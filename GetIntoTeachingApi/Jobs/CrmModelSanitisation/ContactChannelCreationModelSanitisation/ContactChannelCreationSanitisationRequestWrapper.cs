using GetIntoTeachingApi.Models.Crm;
using System;
using System.Collections.ObjectModel;

namespace GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;

/// <summary>
/// Encapsulates the payload required for contact channel sanitisation operations.
/// Holds the candidate's existing contact channel creations and the new channel under review.
/// </summary>
public class ContactChannelCreationSanitisationRequestWrapper
{
    /// <summary>
    /// The read-only collection of previously saved contact channel records for the candidate.
    /// Used to evaluate duplication or merge eligibility.
    /// </summary>
    public ReadOnlyCollection<ContactChannelCreation> CandidateContactChannelCreations { get; private set; }

    /// <summary>
    /// The incoming contact channel creation model to be evaluated and potentially modified.
    /// </summary>
    public ContactChannelCreation CreationChannel { get; private set; }

    /// <summary>
    /// Indicates whether the CreationChannel should be preserved post-sanitisation.
    /// Serves as an explicit signal for downstream logic.
    /// </summary>
    public bool Preserve { get; private set; }

    /// <summary>
    /// Initializes the wrapper with an incoming channel and historical context.
    /// Defaults to non-preservation, requiring explicit logic to toggle intent.
    /// </summary>
    public ContactChannelCreationSanitisationRequestWrapper(
        ContactChannelCreation creationChannel,
        ReadOnlyCollection<ContactChannelCreation> candidateContactChannelCreations)
    {
        CandidateContactChannelCreations = candidateContactChannelCreations;
        CreationChannel = creationChannel;
        Preserve = false;
    }

    /// <summary>
    /// Nullifies the CreationChannel model and flags it as not to be preserved.
    /// Implies the channel is a duplicate or invalid.
    /// </summary>
    public void RemoveCreationChannel()
    {
        CreationChannel = null;
        Preserve = false;
    }

    /// <summary>
    /// Marks the CreationChannel as eligible for preservation.
    /// Used when duplication checks pass or the channel is deemed valid.
    /// </summary>
    public void PreserveCreationChannel()
    {
        Preserve = true;
    }

    /// <summary>
    /// Factory method to create and validate an instance of the wrapper.
    /// Enforces null guards on required input data.
    /// </summary>
    public static ContactChannelCreationSanitisationRequestWrapper Create(
        ContactChannelCreation creationChannel,
        ReadOnlyCollection<ContactChannelCreation> candidateContactChannelCreations)
    {
        ArgumentNullException.ThrowIfNull(creationChannel);
        ArgumentNullException.ThrowIfNull(candidateContactChannelCreations);

        return new ContactChannelCreationSanitisationRequestWrapper(creationChannel, candidateContactChannelCreations);
    }
}
