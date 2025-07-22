using GetIntoTeachingApi.Models.Crm;
using System;
using System.Collections.Generic;
namespace GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;


public class ContactChannelCreationSanitisationRulesHandler : ICrmModelSanitisationRulesHandler<ContactChannelCreationSanitisationRequestWrapper>
{
    private readonly IEnumerable<ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>> _sanitisationRules;


    public ContactChannelCreationSanitisationRulesHandler(IEnumerable<ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>> sanitisationRules)
    {
        _sanitisationRules = sanitisationRules;
    }
    
    
    
    
    public ContactChannelCreationSanitisationRequestWrapper SanitiseCrmModelWithRules(ContactChannelCreationSanitisationRequestWrapper model)
    {
        ArgumentNullException.ThrowIfNull(model);

        foreach (ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper> candidateSanitisationRule in _sanitisationRules)
        {
            model = candidateSanitisationRule.SanitiseCrmModel(model);
        }

        return model;
    }
}
