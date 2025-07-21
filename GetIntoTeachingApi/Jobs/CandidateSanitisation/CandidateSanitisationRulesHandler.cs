using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Models.Crm;
using Hangfire.Common;


namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;

public class CandidateSanitisationRulesHandler : ICandidateSanitisationRulesHandler
{
    private readonly IEnumerable<ICandidateSanitisationRule> _sanitisationRules;
    
    public CandidateSanitisationRulesHandler(IEnumerable<ICandidateSanitisationRule> sanitisationRules)
    {
        _sanitisationRules = sanitisationRules;
    }

    public Candidate SanitiseCandidateWithRules(Candidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        
        foreach (ICandidateSanitisationRule candidateSanitisationRule in _sanitisationRules.ToList())
        {
            candidate = candidateSanitisationRule.SanitiseCandidate(candidate);
        }
        return candidate;
    }
}