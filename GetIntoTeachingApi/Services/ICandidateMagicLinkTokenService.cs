using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApi.Services
{
    public interface ICandidateMagicLinkTokenService
    {
        void GenerateToken(Candidate candidate);
        CandidateMagicLinkExchangeResult Exchange(string token);
    }
}
