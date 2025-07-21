namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;
using GetIntoTeachingApi.Models.Crm;

public interface ICandidateSanitisationRulesHandler
{
    Candidate SanitiseCandidateWithRules(Candidate candidate);
}