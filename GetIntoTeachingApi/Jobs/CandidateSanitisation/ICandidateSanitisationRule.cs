namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;
using GetIntoTeachingApi.Models.Crm;

public interface ICandidateSanitisationRule
{
    Candidate SanitiseCandidate(Candidate updateCandidate);
}