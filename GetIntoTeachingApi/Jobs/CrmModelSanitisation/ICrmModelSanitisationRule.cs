namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;
using GetIntoTeachingApi.Models.Crm;

public interface ICrmModelSanitisationRule<TCrmModel> where TCrmModel : class
{
    TCrmModel SanitiseCrmModel(TCrmModel model);
}