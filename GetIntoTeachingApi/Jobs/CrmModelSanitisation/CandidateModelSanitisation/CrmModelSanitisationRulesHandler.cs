using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Models.Crm;
using Hangfire.Common;


namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;

public class CrmModelSanitisationRulesHandler : ICrmModelSanitisationRulesHandler<Candidate>
{
    private readonly IEnumerable<ICrmModelSanitisationRule<Candidate>> _sanitisationRules;
    
    public CrmModelSanitisationRulesHandler(IEnumerable<ICrmModelSanitisationRule<Candidate>> sanitisationRules)
    {
        _sanitisationRules = sanitisationRules;
    }

    public Candidate SanitiseCrmModelWithRules(Candidate model)
    {
        ArgumentNullException.ThrowIfNull(model);
        
        foreach (ICrmModelSanitisationRule<Candidate> candidateSanitisationRule in _sanitisationRules.ToList())
        {
            model = candidateSanitisationRule.SanitiseCrmModel(model);
        }
        return model;
    }
}