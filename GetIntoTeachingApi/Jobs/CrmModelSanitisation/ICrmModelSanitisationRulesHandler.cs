namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;
using GetIntoTeachingApi.Models.Crm;

public interface ICrmModelSanitisationRulesHandler<TCrmModel> where TCrmModel : class
{
    TCrmModel SanitiseCrmModelWithRules(TCrmModel model);
}