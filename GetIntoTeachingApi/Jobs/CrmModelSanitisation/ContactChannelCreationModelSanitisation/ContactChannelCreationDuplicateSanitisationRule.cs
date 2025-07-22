using GetIntoTeachingApi.Models.Crm;
using System;
using System.Linq;

namespace GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;

/// <summary>
/// Represents a sanitisation rule for <see cref="ContactChannelCreation"/> CRM records.
/// </summary>
public class ContactChannelCreationDuplicateSanitisationRule : ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>
{
   
    public ContactChannelCreationSanitisationRequestWrapper SanitiseCrmModel(
        ContactChannelCreationSanitisationRequestWrapper model)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(model.CreationChannel);
        
        // Check if there is a matching CreationChannel record in CandidateContactChannelCreations
        // If so remove the CreationChannel record, otherwise preserve the CreationChannel record
        if (HasMatchingCreationChannel(model))
        {
            model.RemoveCreationChannel();
            return model;
        }
        
        model.PreserveCreationChannel();
        return model;
    }


    private static bool HasMatchingCreationChannel(ContactChannelCreationSanitisationRequestWrapper model)
    {
        ArgumentNullException.ThrowIfNull(model);

        // TODO: will null match, i.e. does null == null ?
        
        return model.CandidateContactChannelCreations.Any(creationChannel =>
            creationChannel.CreationChannelSourceId == model.CreationChannel.CreationChannelSourceId &&
            creationChannel.CreationChannelServiceId == model.CreationChannel.CreationChannelServiceId &&
            creationChannel.CreationChannelActivityId == model.CreationChannel.CreationChannelActivityId);
    }
}