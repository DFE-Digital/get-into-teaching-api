using GetIntoTeachingApi.Models.Crm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;

public class ContactChannelCreationSanitisationRequestWrapper
{
    public ReadOnlyCollection<ContactChannelCreation> CandidateContactChannelCreations { get; private set; }
    public ContactChannelCreation CreationChannel { get; private set; }
    

    public bool Preserve { get; private set; }


    public ContactChannelCreationSanitisationRequestWrapper(ContactChannelCreation creationChannel, ReadOnlyCollection<ContactChannelCreation> candidateContactChannelCreations)
    {
        CandidateContactChannelCreations = candidateContactChannelCreations;
        CreationChannel = creationChannel;
        Preserve = false;
    }

    public void RemoveCreationChannel()
    {
        CreationChannel = null;
        Preserve = false;
    }

    public void PreserveCreationChannel()
    {
        Preserve = true;
    }

    public static ContactChannelCreationSanitisationRequestWrapper Create(ContactChannelCreation creationChannel,
        ReadOnlyCollection<ContactChannelCreation> candidateContactChannelCreations)
    {
        return new ContactChannelCreationSanitisationRequestWrapper(creationChannel, candidateContactChannelCreations);
    }
}